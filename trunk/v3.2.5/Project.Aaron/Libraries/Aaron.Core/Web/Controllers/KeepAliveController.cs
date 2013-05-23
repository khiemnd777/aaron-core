using System.Web.Mvc;

namespace Aaron.Core.Web.Controllers
{
    public class KeepAliveController : Controller
    {
        public ActionResult Index()
        {
            return Content("I am alive! Haha..!!");
        }
    }
}
