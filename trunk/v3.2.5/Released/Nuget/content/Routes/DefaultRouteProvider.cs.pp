namespace $rootnamespace$.Routes
{
    using System.Web.Mvc;
    using System.Web.Routing;
    using Aaron.Core.Web.Routes;
    using Aaron.Core.Web.Localization;

    public class DefaultRouteProvider : IRouterProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapLocalizedRoute("HomePage", // Route name
                "", // URL with parameters
                new { controller = "Home", action = "Index"}, // Parameter defaults
                new[] { "$rootnamespace$.Controllers" }
                );

            //topic
            routes.MapLocalizedRoute("Topic",
                "{SystemName}",
                new { controller = "Topic", action = "TopicDetails" },
                new[] { "$rootnamespace$.Controllers" });

            routes.MapLocalizedRoute("TopicAuthenticate",
                "topic/authenticate",
                new { controller = "Topic", action = "Authenticate" },
                new[] { "$rootnamespace$.Controllers" });

            //robots.txt
            routes.MapLocalizedRoute("robots.txt",
                "robots.txt",
                new { controller = "Common", action = "RobotsTextFile" },
                new[] { "$rootnamespace$.Controllers" });
        }

        public int Order
        {
            get { return 1; }
        }
    }
}