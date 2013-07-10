using System.Web.Mvc;
using ClientDAL;
using Services;
using ShardingMvcDemo.Raven;

namespace ShardingMvcDemo.Controllers
{
    public class DeleteController : Controller
    {
        public virtual ActionResult Index(string id)
        {
            using (var ravenDbConnection = new RavenDbConnection(new RavenDbConnectionManager()))
            {
                var service = new ClientService(ravenDbConnection);
                service.DeleteClient(id);
            }

            return RedirectToAction("Index", "Home");
        }

    }
}
