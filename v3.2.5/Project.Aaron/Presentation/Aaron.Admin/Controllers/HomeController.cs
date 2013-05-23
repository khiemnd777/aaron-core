using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aaron.Core.Web.Security;

namespace Aaron.Admin.Controllers
{
    [AdminAuthorize]
    public class HomeController : BaseController
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }
    }
}
