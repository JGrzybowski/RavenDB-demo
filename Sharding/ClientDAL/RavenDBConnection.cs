using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ClientModel;
using OrderModel;
using Raven.Abstractions.Commands;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Listeners;
using Raven.Client.Shard;
using Raven.Json.Linq;


namespace ClientDAL
{
    public class RavenDbConnection : IDisposable
    {
        #region Constructor
        public RavenDbConnection(IRavenDbConnectionManager connectionManager)
        {
            var servers = connectionManager.GetConnectionStrings();
            _nrShards = servers.Count;
            _shards = new Dictionary<string, IDocumentStore>();
            foreach (string regionName in servers)
            {
                _shards.Add(regionName, new DocumentStore { ConnectionStringName = regionName });
            }
            var shardStrategy = new ShardStrategy(_shards)
                .ShardingOn<Client>(client => client.Region);
            _clientsDocumentStore = new ShardedDocumentStore(shardStrategy).Initialize();

            servers = connectionManager.GetReplicationStrings();
            foreach (var replicationString in servers)
            {
                _replicationStores.Add(new DocumentStore() { ConnectionStringName = replicationString }.Initialize());
            }
            foreach (var replicationStore in _replicationStores)
            {
                replicationStore.Conventions.FailoverBehavior =
                    FailoverBehavior.AllowReadsFromSecondariesAndWritesToSecondaries;
            }
        }
        public void Dispose()
        {
            _clientsDocumentStore.Dispose();
        }
        #endregion

        #region Fields
        private int _nrShards = 0;
        private new Dictionary<string, IDocumentStore> _shards = null;
        private List<IDocumentStore> _replicationStores = new List<IDocumentStore>();
        private readonly IDocumentStore _clientsDocumentStore;
        private Random _gen = new Random(DateTime.Now.Millisecond);
        private const int MaxStorePerSession = 10000;
        #endregion

        #region Managing the Clients DB
        #region Adding

        public void AddClient(Client client)
        {
            using (var session = _clientsDocumentStore.OpenSession())
            {
                session.Store(client);
                session.SaveChanges();
            }
        }
        public void AddClients(IEnumerable<Client> list)
        {
            //TODO zabezpieczyć na wypadek zbyt dużych zapytań
            using (var session = _clientsDocumentStore.OpenSession())
            {
                foreach (var client in list)
                {
                    session.Store(client);
                }

                session.SaveChanges();
            }
        }

        #endregion

        #region Deleting

        public void DeleteClient(string id)
        {
            using (var session = _clientsDocumentStore.OpenSession())
            {
                session.Advanced.Defer(new DeleteCommandData { Key = id });
                session.SaveChanges();
            }
        }
        public void DeleteClients(IEnumerable<string> idList)
        {
            using (var session = _clientsDocumentStore.OpenSession())
            {
                foreach (string id in idList)
                    session.Advanced.Defer(new DeleteCommandData { Key = id });
                session.SaveChanges();
            }
        }
        public void DeleteAllClients()
        {
            List<Client> results = new List<Client>();
            do
            {
                using (var session = _clientsDocumentStore.OpenSession())
                {
                    results = session.Query<Client>().Take(1024).ToList();
                    foreach (Client client in results)
                        session.Advanced.Defer(new DeleteCommandData { Key = client.Id });
                    session.SaveChanges();
                }
            } while (results.Count != 0);
        }

        #endregion

        #region Updating

        public void UpdateClient(string id, string name, string surname, string country, string region)
        {
            using (var session = _clientsDocumentStore.OpenSession())
            {
                var existingClient = session.Load<Client>(id);
                existingClient.FirstName = name;
                existingClient.LastName = surname;
                existingClient.Country = country;
                existingClient.Region = region;
                existingClient.ModificationDateTime = DateTime.Now;
                session.SaveChanges();
            }
        }
        public void UpdateClient(string id, string name, string surname, string country, string region, DateTime dateTime)
        {
            using (var session = _clientsDocumentStore.OpenSession())
            {
                var existingClient = session.Load<Client>(id);
                existingClient.FirstName = name;
                existingClient.LastName = surname;
                existingClient.Country = country;
                existingClient.Region = region;
                existingClient.ModificationDateTime = dateTime;
                session.SaveChanges();
            }
        }
        public void UpdateClientFirstName(string id, string firstName)
        {
            using (var session = _clientsDocumentStore.OpenSession())
            {
                var existingClient = session.Load<Client>(id);
                existingClient.FirstName = firstName;
                existingClient.ModificationDateTime = DateTime.Now;
                session.SaveChanges();
            }
        }
        public void UpdateClientLastName(string id, string lastName)
        {
            using (var session = _clientsDocumentStore.OpenSession())
            {
                var existingClient = session.Load<Client>(id);
                existingClient.LastName = lastName;
                existingClient.ModificationDateTime = DateTime.Now;
                session.SaveChanges();
            }
        }
        public void UpdateClientCountry(string id, string country)
        {
            using (var session = _clientsDocumentStore.OpenSession())
            {
                var existingClient = session.Load<Client>(id);
                existingClient.Country = country;
                existingClient.ModificationDateTime = DateTime.Now;
                session.SaveChanges();
            }
        }
        public void UpdateClientRegion(string id, string region)
        {
            using (var session = _clientsDocumentStore.OpenSession())
            {
                var existingClient = session.Load<Client>(id);
                existingClient.Region = region;
                existingClient.ModificationDateTime = DateTime.Now;
                session.SaveChanges();
            }
        }

        #endregion

        #region Reading

        public Client GetClient(string id)
        {
            using (var session = _clientsDocumentStore.OpenSession())
            {
                var result = session.Load<Client>(id);
                return result;
            }
        }
        /*
         * http://ayende.com/blog/155361/ravendb-sharding-map-reduce-in-a-cluster
         * GetClients musi pobierać więcej danych, niż się wydaje ze względu na podział danych
         * Komenda .Skip(n) pomija n pierwszych wyników z każdego sharda/servera, wiec nie jest tu przydatny
         */
        public ShardedResults<Client> GetClients(int page, int itemsPerPage)
        {
            var results = new ShardedResults<Client>();
            var query = new List<Client>();
            Parallel.ForEach(_shards, pair =>
            {
                try
                {
                    int limit = itemsPerPage*page;
                    while (limit > 0)
                    {
                        using (var session = pair.Value.OpenSession())
                        {
                            query.AddRange(session.Query<Client>()
                                .OrderByDescending(client => client.ModificationDateTime)
                                .Take(limit)
                                .ToArray());
                        }
                        limit -= Math.Min(itemsPerPage, RavenDBConst.MaxPageSize);
                    }
                }
                catch (WebException webException)
                {
                    results.ErrorConnectionStrings.Add(pair.Key);
                }
            }
            );
            results.Results = query
                .OrderByDescending(c => c.ModificationDateTime)
                .Skip((page - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .ToList();
            return results;
        }

        #endregion
        #endregion

        #region Managing the Orders DB
        #region Picking one of the replications servers

        private IDocumentStore GetReplicationStore()
        {
            return _replicationStores[_gen.Next(_replicationStores.Count)];
        }

        #endregion

        #region Adding

        public void AddOrder(Order order)
        {
            using (var session = GetReplicationStore().OpenSession())
            {
                session.Store(order);
                session.SaveChanges();
            }
        }
        public void AddOrders(List<DummyOrder> list)
        {
            var commands = new List<ICommandData>();
            foreach (var order in list)
            {
                commands.Add(new PutCommandData()
                {
                    Document = RavenJObject.FromObject(order),
                    Etag = null,
                    Key = "orders/",
                    Metadata = new RavenJObject()
                });
            }
            var batchResults = GetReplicationStore().DatabaseCommands.Batch(commands);
        }
        public void AddOrders(List<Order> list)
        {
            int limit = 0;
           
            while (limit++ < Math.Round((double)(list.Count / MaxStorePerSession)))
            {
                using (var session = GetReplicationStore().OpenSession())
                {
                    foreach (var order in list.Skip(MaxStorePerSession * (limit - 1)).Take(MaxStorePerSession))
                    {
                        session.Store(order);
                    }
                    session.SaveChanges();
                }
            }
        }

        #endregion
        
        #region Deleting

        public void DeleteOrder(string id)
        {
            using (var session = GetReplicationStore().OpenSession())
            {
                var existingOrder = session.Load<Order>(id);
                session.Delete(existingOrder);
                session.SaveChanges();
            }
        }
        public void DeleteOrders(IEnumerable<string> idList)
        {
            using (var session = GetReplicationStore().OpenSession())
            {
                var results = session.Query<Order>().Where(order => idList.ToList().Contains(order.Id)).ToList();
                foreach (var order in results)
                {
                    session.Advanced.Defer(new DeleteCommandData { Key = order.Id });
                }
                session.SaveChanges();
            }
        }
        public void DeleteAllOrders()
        {
            List<Order> results = null;
            do
            {
                using (var session = GetReplicationStore().OpenSession())
                {
                    results = session.Query<Order>().Take(1024).ToList();
                    foreach (Order order in results)
                        session.Advanced.Defer(new DeleteCommandData { Key = order.Id });
                    session.SaveChanges();
                }
            } while (results.Count != 0);
        }

        #endregion
        
        #region Updating

        public void UpdateOrder(string orderId, string clientId, List<Payment> payments, List<Product> products, DateTime timeOfOrder)
        {
            using (var session = GetReplicationStore().OpenSession())
            {
                var existingOrder = session.Load<Order>(orderId);
                existingOrder.ClientId = clientId;
                existingOrder.Payments = payments;
                existingOrder.Products = products;
                existingOrder.TimeOfOrder = timeOfOrder;
                session.SaveChanges();
            }
        }
        public void UpdateOrderClientId(string orderId, string clientId)
        {
            using (var session = GetReplicationStore().OpenSession())
            {
                var existingOrder = session.Load<Order>(orderId);
                existingOrder.ClientId = clientId;
                session.SaveChanges();
            }
        }
        public void UpdateOrderPayments(string orderId, List<Payment> payments)
        {
            using (var session = GetReplicationStore().OpenSession())
            {
                var existingOrder = session.Load<Order>(orderId);
                existingOrder.Payments = payments;
                session.SaveChanges();
            }
        }
        public void UpdateOrderProducts(string orderId, List<Product> products)
        {
            using (var session = GetReplicationStore().OpenSession())
            {
                var existingOrder = session.Load<Order>(orderId);
                existingOrder.Products = products;
                session.SaveChanges();
            }
        }
        public void UpdateOrderTime(string orderId, DateTime timeOfOrder)
        {
            using (var session = GetReplicationStore().OpenSession())
            {
                var existingOrder = session.Load<Order>(orderId);
                existingOrder.TimeOfOrder = timeOfOrder;
                session.SaveChanges();
            }
        }

        public void AddPayment(string orderId, Payment payment)
        {
            using (var session = GetReplicationStore().OpenSession())
            {
                var existingOrder = session.Load<Order>(orderId);
                existingOrder.Payments.Add(payment);
                session.SaveChanges();
            }
        }
        public void AddPayment(string orderId, string paymentId, double amount)
        {
            using (var session = GetReplicationStore().OpenSession())
            {
                var existingOrder = session.Load<Order>(orderId);
                existingOrder.Payments.Add(new Payment() { Amount = amount, Id = paymentId });
                session.SaveChanges();
            }
        }
        public void AddProduct(string orderId, Product product)
        {
            using (var session = GetReplicationStore().OpenSession())
            {
                var existingOrder = session.Load<Order>(orderId);
                existingOrder.Products.Add(product);
                session.SaveChanges();
            }
        }
        public void AddProduct(string orderId, string name, double price)
        {
            using (var session = GetReplicationStore().OpenSession())
            {
                var existingOrder = session.Load<Order>(orderId);
                existingOrder.Products.Add(new Product() { Name = name, Price = price });
                session.SaveChanges();
            }
        }

        #endregion
        
        #region Reading

        public Order GetOrder(string id)
        {
            Order result = null;
            //using (var session = _replicationStores[0].OpenSession())
            using (var session = GetReplicationStore().OpenSession())
            {
                result = session.Load<Order>(id);
                session.Advanced.GetMetadataFor(result)[Constants.RavenEntityName] = "Orders";
                session.SaveChanges();
            }
            return result;
        }
        public List<Order> GetOrders(int page, int itemsPerPage)
        {
            var results = new List<Order>();
            using (var session = GetReplicationStore().OpenSession())
            {
                results = session.Query<Order>()
                    .OrderByDescending(order => order.TimeOfOrder)
                    .Skip(itemsPerPage * (page - 1))
                    .Take(itemsPerPage)
                    .ToList();
                session.SaveChanges();
            }
            return results;
        }

        #endregion
        #endregion
    }
}
