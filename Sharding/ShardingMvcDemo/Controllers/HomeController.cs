using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ClientDAL;
using Services;
using ShardingMvcDemo.Raven;
using ShardingMvcDemo.ViewModels;

namespace ShardingMvcDemo.Controllers
{
    public class HomeController : Controller
    {
        public virtual ActionResult Index(int currentPage = 1, int itemsPerPage = 15)
        {
            if (currentPage < 1) currentPage = 1;
            if (itemsPerPage < 1) itemsPerPage = 1;

            using (var rdbc = new RavenDbConnection(new RavenDbConnectionManager()))
            {
                var service = new ClientService(rdbc);
                var clients = service.GetClients(currentPage, itemsPerPage);
                var clientViewModels = new List<ClientEditViewModel>(clients.Count);
                clientViewModels.AddRange(clients.Select(client => new ClientEditViewModel(client)));

                var clientHomeViewModel = new ClientHomeViewModel
                {
                    Clients = clientViewModels,
                    CurrentPage = currentPage,
                    ItemsPerPage = itemsPerPage
                };
                return View(clientHomeViewModel);
            }
        }
    }
}
