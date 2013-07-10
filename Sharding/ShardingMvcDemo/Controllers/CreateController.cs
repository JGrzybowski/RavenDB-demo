using System.Web.Mvc;
using ClientDAL;
using Services;
using ShardingMvcDemo.Raven;
using ShardingMvcDemo.ViewModels;

namespace ShardingMvcDemo.Controllers
{
    public class CreateController : Controller
    {
        //
        // GET: /Create/
        public virtual ActionResult Index()
        {
            return View();
        }


        //
        // POST: /Create/
        [HttpPost]
        public virtual ActionResult Index(ClientEditViewModel clientEdit)
        {
            try
            {
                using (var ravenDbConnection = new RavenDbConnection(new RavenDbConnectionManager()))
                {
                    var service = new ClientService(ravenDbConnection);
                    service.AddClient(clientEdit.FirstName, clientEdit.LastName, clientEdit.Country);
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
