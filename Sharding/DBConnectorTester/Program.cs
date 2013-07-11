using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ClientModel;
using ClientDAL;
using OrderModel;

namespace DBConnectorTester
{
    class Program
    {
        #region random data generators
        private static DateTime _start = new DateTime(2000, 1, 1);
        private static Random gen = new Random();
        private static int range = (DateTime.Today - _start).Days;

        private static Dictionary<string, string> _countries = new Dictionary<string, string>()
            {
                {"France", "EU"},
                {"Germany", "EU"},
                {"USA", "USA"},
                {"Norway", "Rest"},
                {"Peru", "Rest"}
            };
        private static Dictionary<string, List<string>> _names = new Dictionary<string, List<string>>()
            {
                {"France",  new List<string>(){"Jean","Pierre","Marie","Rita"}},
                {"Germany", new List<string>(){"Anita","Deniz","Franziska","Hans"}},
                {"USA",     new List<string>(){"John","Amy","Chris","Bob"}},
                {"Norway",  new List<string>(){"Albert","Erhard","Jarand","Eirin"}},
                {"Peru",    new List<string>(){"Avelino","Ciriaco","Felipe","Maribel"}}
            };
        private static Dictionary<string, List<string>> _lastNames = new Dictionary<string, List<string>>()
            {
                {"France",  new List<string>(){"Daquin","Levittoux","Pourbaix","Bergeron"}},
                {"Germany", new List<string>(){"Angerer","Kisch","Jauch","Zwick"}},
                {"USA",     new List<string>(){"Doe","Whitener","Jenkins","Freeman"}},
                {"Norway",  new List<string>(){"Larsen","Solberg","Northug","Dahl"}},
                {"Peru",    new List<string>(){"Medina","Angulo","Sanches","Gallegos"}}
            };

        private static List<string> _products = new List<string>() { "Cable", "Mouse", "Keyboard", "Monitor", "Camera", "Mainboard", "Graphics Card", "CPU" };
        private static string[] _banks = { "PKO", "ING", "Pekao", "Millennium", "BGŻ" };
        private static int[] _transactions = { 0, 0, 0, 0, 0 };

        public static List<Client> RandomClients(int nrClients)
        {
            var result = new List<Client>();
            DateTime time = _start.AddDays(gen.Next(range));
            List<string> name;
            List<string> lastName;
            for (int i = 0; i < nrClients; ++i)
            {
                var country = _countries.Keys.ToArray()[gen.Next(_countries.Keys.Count)];

                string region;
                if (!_names.TryGetValue(country, out name))
                    name = new List<string> { "unknown" };
                if (!_lastNames.TryGetValue(country, out lastName))
                    lastName = new List<string> { "unknown" };
                if (!_countries.TryGetValue(country, out region))
                    region = "unknown";
                var client = new Client(name[gen.Next(name.Count)], lastName[gen.Next(lastName.Count)], country, region);
                client.ModificationDateTime = _start.AddDays(gen.Next(range));
                result.Add(client);
            }
            return result;
        }

        public static List<Order> RandomOrders(int nrOrders, RavenDbConnection dbConnection)
        {
            var result = new List<Order>();
            DateTime time = _start.AddDays(gen.Next(range));
            var clientsList = dbConnection.GetClients(1, 15);
            for (int i = 0; i < nrOrders; ++i)
            {
                var client = clientsList.Results[gen.Next(clientsList.Results.Count)];
                result.Add(new Order(client.Id, client.FirstName, client.LastName, client.Country, null, null, DateTime.Now));
            }
            foreach (var order in result)
            {
                int nrProducts = gen.Next(1, 5);
                for (int i = 0; i < nrProducts; ++i)
                    order.AddProduct(_products[gen.Next(_products.Count)], Math.Round(gen.NextDouble() * 300, 2));
                int bankId = gen.Next(_banks.Count());
                order.AddPayment(_banks[bankId] + "/" + (++_transactions[bankId]), Math.Round(gen.NextDouble() * -order.Balance, 2));
                order.AddPayment(_banks[bankId] + "/" + (++_transactions[bankId]), Math.Round(gen.NextDouble() * -order.Balance, 2));
            }
            return result;

        }
        public static List<DummyOrder> RandomDummyOrders(int nrOrders, RavenDbConnection dbConnection)
        {
            var result = new List<DummyOrder>();
            DateTime time = _start.AddDays(gen.Next(range));
            var clientsList = dbConnection.GetClients(1, 15).Results;
            for (int i = 0; i < nrOrders; ++i)
            {
                var client = clientsList[gen.Next(clientsList.Count)];
                result.Add(new DummyOrder(client.Id, client.FirstName, client.LastName, client.Country, null, null, DateTime.Now));
            }
            foreach (var order in result)
            {
                int nrProducts = gen.Next(1, 5);
                for (int i = 0; i < nrProducts; ++i)
                    order.AddProduct(_products[gen.Next(_products.Count)], Math.Round(gen.NextDouble() * 300,2));
                int bankId = gen.Next(_banks.Count());
                order.AddPayment(_banks[bankId] + "/" + (++_transactions[bankId]), Math.Round(gen.NextDouble() * -order.Balance,2));
                order.AddPayment(_banks[bankId] + "/" + (++_transactions[bankId]), Math.Round(gen.NextDouble() * -order.Balance,2));
            }
            return result;

        }
        #endregion

        private static void Main(string[] args)
        {
            var manager = new ConnectionManager();
            var dbConnector = new RavenDbConnection(manager);
            const bool _populating = false;
            const int _nrClients = 100000;
            const bool _reading = true;
            const int _page = 6;
            const int _itemsPerPage = 10000;
            const bool _deleting = false;

            const bool _order = false;

            #region Populate ClientsDB
            if (_populating)
            {
                var clients = RandomClients(_nrClients);
                DateTime time = _start.AddDays(gen.Next(range));
                dbConnector.AddClients(clients);
            }

            #endregion
            #region Reading the data base

            if (_reading)
            {
                ShardedResults<Client> list = dbConnector.GetClients(_page, _itemsPerPage);
                foreach (var client in list.Results)
                {
                    Console.WriteLine(client.ToString());
                }
                if (list.ErrorConnectionStrings.Count > 0)
                {
                    Console.WriteLine("Failed Connections");
                    foreach (var conectionStringName in list.ErrorConnectionStrings)
                    {
                        Console.WriteLine(conectionStringName);
                    }
                }
            }
            #endregion
            #region Clearing the data base

            if (_deleting)
            {
                //ShardedResults<Client> list = dbConnector.GetClients(_page, _itemsPerPage);
                dbConnector.DeleteAllClients();
            }

            #endregion

            #region Simulating several orders

            if (_order)
            {
                var order = new Order("Rest/clients/671603", "Albert", "Solberg", "Norway", null, null, DateTime.Now);
                order.AddProduct("Mouse", 15.50);
                order.AddProduct("Cable", 3.25);
                order.AddPayment("PKO-01", 17.75);
                dbConnector.AddOrder(order);
            }

            //var orderList = RandomOrders(90000,dbConnector);
            //dbConnector.AddOrders(orderList);
            var example = dbConnector.GetOrder("orders/250000");
            //var olist = dbConnector.GetOrders(2, 15);
            //dbConnector.DeleteAllOrders();
            #endregion

            Console.Write("End of program. Press any key to exit the application...");
            Console.ReadKey();
        }
    }
    class ConnectionManager : IRavenDbConnectionManager
    {
        public List<string> GetConnectionStrings()
        {
            return new List<string> { "EU", "USA", "Rest" };
        }
        public List<string> GetReplicationStrings()
        {
            return new List<string> { "Orders1", "Orders2" };
        }

    }

}
