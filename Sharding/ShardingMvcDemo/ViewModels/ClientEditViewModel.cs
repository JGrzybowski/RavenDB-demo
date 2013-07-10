using System.ComponentModel.DataAnnotations;
using ClientModel;

namespace ShardingMvcDemo.ViewModels
{
    public class ClientEditViewModel
    {
        public string Id { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        public string Country { get; set; }

        public ClientEditViewModel(Client client)
        {
            Id = client.Id;
            FirstName = client.FirstName;
            LastName = client.LastName;
            Country = client.Country;
        }

        public ClientEditViewModel()
        {
        }
    }
}