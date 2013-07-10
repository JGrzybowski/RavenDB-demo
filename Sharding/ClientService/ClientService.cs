using System;
using System.Collections.Generic;
using System.Linq;
using ClientDAL;
using ClientModel;
using Raven.Client;
using Raven.Client.Document;

namespace Services
{
    public class ClientService
    {
        private readonly RavenDbConnection _ravenDbConnection;

        private readonly IDocumentStore _countriesDocumentStore = new DocumentStore
        {
            ConnectionStringName = "CountriesRegions"
        };

        private readonly Dictionary<string, string> _countriesRegions = new Dictionary<string, string>();

        public ClientService(RavenDbConnection ravenDbConnection)
        {
            _ravenDbConnection = ravenDbConnection;
            _countriesDocumentStore.Initialize();
            _countriesRegions.Add("UK", "EU");
            _countriesRegions.Add("USA", "USA");
            _countriesRegions.Add("Sweden", "EU");
            _countriesRegions.Add("Spain", "EU");
            _countriesRegions.Add("Slovenia", "EU");
            _countriesRegions.Add("Slovakia", "EU");
            _countriesRegions.Add("Romania", "EU");
            _countriesRegions.Add("Portugal", "EU");
            _countriesRegions.Add("Poland", "EU");
            _countriesRegions.Add("Netherlands", "EU");
            _countriesRegions.Add("Malta", "EU");
            _countriesRegions.Add("Luxembourg", "EU");
            _countriesRegions.Add("Lithuania", "EU");
            _countriesRegions.Add("Latvia", "EU");
            _countriesRegions.Add("Italy", "EU");
            _countriesRegions.Add("Ireland", "EU");
            _countriesRegions.Add("Hungary", "EU");
            _countriesRegions.Add("Greece", "EU");
            _countriesRegions.Add("Germany", "EU");
            _countriesRegions.Add("France", "EU");
            _countriesRegions.Add("Finland", "EU");
            _countriesRegions.Add("Estonia", "EU");
            _countriesRegions.Add("Denmark", "EU");
            _countriesRegions.Add("Czech Republic", "EU");
            _countriesRegions.Add("Cyprus", "EU");
            _countriesRegions.Add("Croatia", "EU");
            _countriesRegions.Add("Bulgaria", "EU");
            _countriesRegions.Add("Belgium", "EU");
            _countriesRegions.Add("Austria", "EU");
        }

        public void AddClient(string name, string lastName, string country)
        {
            string region;
            if (!_countriesRegions.TryGetValue(country, out region))
            {
                region = "Rest";
            }
            var newClient = new Client(name, lastName, country, region);
            _ravenDbConnection.AddClient(newClient);
        }

        public void DeleteClient(string id)
        {
            _ravenDbConnection.DeleteClient(id);
        }

        public Client GetClient(string id)
        {
            return _ravenDbConnection.GetClient(id);
        }

        public List<Client> GetClients(int page, int itemsPerPage)
        {
            return _ravenDbConnection.GetClients(page, itemsPerPage);
        }

        /*
         * Jak robić update? Wszystko naraz czy tak jak jest teraz?
         */

        public void UpdateClientCountry(string id, string newCountry)
        {
            _ravenDbConnection.UpdateClientCountry(id, newCountry);
        }

        public void UpdateClientFirstName(string id, string newFirstName)
        {
            _ravenDbConnection.UpdateClientFirstName(id, newFirstName);
        }

        public void UpdateClientLastName(string id, string newLastName)
        {
            _ravenDbConnection.UpdateClientLastName(id, newLastName);
        }

        public void UpdateClient(String id, string newFirstName, string newLastName, string newCountry)
        {
            string region;
            if (!_countriesRegions.TryGetValue(newCountry, out region))
            {
                region = "Rest";
            }
            _ravenDbConnection.UpdateClient(id, newFirstName, newLastName, newCountry, region);
        }

        public class CountryRegion
        {
            public string Id { get; set; }
            public string Country { get; set; }
            public string Region { get; set; }

            public CountryRegion()
            {
            }
        }
    }
}
