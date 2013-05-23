using System.Web.Mvc;
using System.Web.Routing;
using Aaron.Core.Web.Routes;

namespace Aaron.Plugin.ExternalAuth.Facebook
{
    public partial class RouteProvider : IRouterProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.ExternalAuth.Facebook.Configure",
                 "Plugins/ExternalAuthFacebook/Configure",
                 new { controller = "ExternalAuthFacebook", action = "Configure" },
                 new[] { "Aaron.Plugin.ExternalAuth.Facebook.Controllers" }
            );

            routes.MapRoute("Plugin.ExternalAuth.Facebook.PublicInfo",
                 "Plugins/ExternalAuthFacebook/PublicInfo",
                 new { controller = "ExternalAuthFacebook", action = "PublicInfo" },
                 new[] { "Aaron.Plugin.ExternalAuth.Facebook.Controllers" }
            );

            routes.MapRoute("Plugin.ExternalAuth.Facebook.Login",
                 "Plugins/ExternalAuthFacebook/Login",
                 new { controller = "ExternalAuthFacebook", action = "Login" },
                 new[] { "Aaron.Plugin.ExternalAuth.Facebook.Controllers" }
            );
        }


        public int Order
        {
            get { return 1; }
        }
    }
}
