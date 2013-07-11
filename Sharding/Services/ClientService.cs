using System;
using System.Collections.Generic;
using ClientDAL;
using ClientModel;
using Raven.Client;
using Raven.Client.Document;

namespace Services
{
    public class ClientService
    {
        private readonly RavenDbConnection _ravenDbConnection;

        private readonly Dictionary<string, string> _dictCountriesRegions = new Dictionary<string, string>();

        public ClientService(RavenDbConnection ravenDbConnection)
        {
            _ravenDbConnection = ravenDbConnection;
            initDictCountriesRegions();
        }

        public void AddClient(string name, string lastName, string country)
        {
            var region = getRegion(country);
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

        public ShardedResults<Client> GetClients(int page, int itemsPerPage)
        {
            return _ravenDbConnection.GetClients(page, itemsPerPage);
        }

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
            string region = getRegion(newCountry);
            _ravenDbConnection.UpdateClient(id, newFirstName, newLastName, newCountry, region);
        }

        private string getRegion(string country)
        {
            string region;
            if (!_dictCountriesRegions.TryGetValue(country, out region))
            {
                region = "Rest";
            }
            return region;
        }

        private void initDictCountriesRegions()
        {
            _dictCountriesRegions.Add("UK", "EU");
            _dictCountriesRegions.Add("USA", "USA");
            _dictCountriesRegions.Add("Sweden", "EU");
            _dictCountriesRegions.Add("Spain", "EU");
            _dictCountriesRegions.Add("Slovenia", "EU");
            _dictCountriesRegions.Add("Slovakia", "EU");
            _dictCountriesRegions.Add("Romania", "EU");
            _dictCountriesRegions.Add("Portugal", "EU");
            _dictCountriesRegions.Add("Poland", "EU");
            _dictCountriesRegions.Add("Netherlands", "EU");
            _dictCountriesRegions.Add("Malta", "EU");
            _dictCountriesRegions.Add("Luxembourg", "EU");
            _dictCountriesRegions.Add("Lithuania", "EU");
            _dictCountriesRegions.Add("Latvia", "EU");
            _dictCountriesRegions.Add("Italy", "EU");
            _dictCountriesRegions.Add("Ireland", "EU");
            _dictCountriesRegions.Add("Hungary", "EU");
            _dictCountriesRegions.Add("Greece", "EU");
            _dictCountriesRegions.Add("Germany", "EU");
            _dictCountriesRegions.Add("France", "EU");
            _dictCountriesRegions.Add("Finland", "EU");
            _dictCountriesRegions.Add("Estonia", "EU");
            _dictCountriesRegions.Add("Denmark", "EU");
            _dictCountriesRegions.Add("Czech Republic", "EU");
            _dictCountriesRegions.Add("Cyprus", "EU");
            _dictCountriesRegions.Add("Croatia", "EU");
            _dictCountriesRegions.Add("Bulgaria", "EU");
            _dictCountriesRegions.Add("Belgium", "EU");
            _dictCountriesRegions.Add("Austria", "EU");
        }

        public class CountryRegion
        {
            public string Id { get; set; }
            public string Country { get; set; }
            public string Region { get; set; }
        }
    }
}
