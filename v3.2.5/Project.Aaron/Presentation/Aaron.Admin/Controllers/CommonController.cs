using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aaron.Core.Web;
using Aaron.Core.Services.Security;
using Aaron.Core.Caching;
using Aaron.Core.Services.Common;

namespace Aaron.Admin.Controllers
{
    public class CommonController : BaseController
    {
        private readonly IWebHelper _webHelper;
        private readonly IPermissionService _permissionService;

        public CommonController(IWebHelper webHelper,
            IPermissionService permissionService)
        {
            _webHelper = webHelper;
            _permissionService = permissionService;
        }

        public ActionResult RestartApp()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AllowRestartApp))
                return AccessDeniedView();

            _webHelper.RestartAppDomain();
            return RedirectToAction("Index", "Home");   
        }

        public ActionResult ClearCache()
        {
            var cache = new MemoryCacheManager();
            cache.Clear();

            return RedirectToAction("Index", "Home");
        }
    }
}
