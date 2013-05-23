using System.Web.Mvc;
using System.Web.Routing;
using Aaron.Core.Web.Routes;
using Aaron.Core.Web.Localization;

namespace Aaron.Core.Services.Common
{
    public class CommonRouteProvider : IRouterProvider
    {
        public void RegisterRoutes(System.Web.Routing.RouteCollection routes)
        {
            // install - success
            routes.MapLocalizedRoute("Install",
                "install",
                new { controller = "Install", action = "Index" },
                new[] { "Aaron.Core.Web.Controllers" });

            routes.MapLocalizedRoute("InstallSuccess",
                "success",
                new { controller = "Install", action = "Success" },
                new[] { "Aaron.Core.Web.Controllers" });
        }

        public int Order
        {
            get { return 2; }
        }
    }
}
