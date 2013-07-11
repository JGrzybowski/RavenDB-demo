using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShardingMvcDemo.ViewModels
{
    public class ClientHomeViewModel
    {
        public List<ClientEditViewModel> Clients { get; set; }

        [Display(Name = "Current page")]
        public int CurrentPage { get; set; }

        [Display(Name = "Items per page")]
        public int ItemsPerPage { get; set; }

        private List<string> _downServersList = new List<string>();

        public List<string> GetDownServersList()
        {
            return _downServersList;
        }

        public void AddDownServers(string server)
        {
            _downServersList.Add(server);
        }
    }
}