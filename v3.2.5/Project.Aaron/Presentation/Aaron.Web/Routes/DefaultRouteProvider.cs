namespace Aaron.Web.Routes
{
    using System.Web.Mvc;
    using System.Web.Routing;
    using Aaron.Core.Web.Routes;
    using Aaron.Core.Web.Localization;

    public class DefaultRouteProvider : IRouterProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.html/{*pathInfo}");

            // Notice:
            // Put all custom MapRoute above default.

            routes.MapLocalizedRoute("HomePage", // Route name
                "", // URL with parameters
                new { controller = "Home", action = "Index"}, // Parameter defaults
                new[] { "Aaron.Web.Controllers" }
                );

            //topic
            routes.MapLocalizedRoute("Topic",
                "{SystemName}",
                new { controller = "Topic", action = "TopicDetails" },
                new[] { "Aaron.Web.Controllers" });

            routes.MapLocalizedRoute("TopicAuthenticate",
                "topic/authenticate",
                new { controller = "Topic", action = "Authenticate" },
                new[] { "Aaron.Web.Controllers" });

            //robots.txt
            routes.MapLocalizedRoute("robots.txt",
                "r/robots.txt",
                new { controller = "Common", action = "RobotsTextFile" },
                new[] { "Aaron.Web.Controllers" });
        }

        public int Order
        {
            get { return 1; }
        }
    }
}