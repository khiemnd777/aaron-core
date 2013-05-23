namespace $rootnamespace$.Routes
{
    using System.Web.Mvc;
    using System.Web.Routing;
    using Aaron.Core.Web.Routes;


    public class DefaultRouteProvider : IRouterProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute(
                "", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );
        }

        public int Order
        {
            get { return 1; }
        }
    }
}