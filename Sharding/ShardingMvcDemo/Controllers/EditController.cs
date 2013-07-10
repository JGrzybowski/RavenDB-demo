using System.Web.Mvc;
using ClientDAL;
using Services;
using ShardingMvcDemo.Raven;
using ShardingMvcDemo.ViewModels;

namespace ShardingMvcDemo.Controllers
{
    public class EditController : Controller
    {
        //
        // GET: /Edit/

        public ActionResult Index(string id)
        {
            using (var ravenDbConnection = new RavenDbConnection(new RavenDbConnectionManager()))
            {
                var service = new ClientService(ravenDbConnection);
                var client = service.GetClient(id);
                var viewModel = new ClientEditViewModel(client);
                return View(viewModel);
            }
        }

        [HttpPost]
        public virtual ActionResult Index(ClientEditViewModel clientEditViewModel)
        {
            using (var ravenDbConnection = new RavenDbConnection(new RavenDbConnectionManager()))
            {
                var service = new ClientService(ravenDbConnection);
                service.UpdateClient(clientEditViewModel.Id, clientEditViewModel.FirstName, clientEditViewModel.LastName, clientEditViewModel.Country);
                return RedirectToAction("Index", "Home");
            }
        }

    }
}
