using System.Web.Routing;

namespace Aaron.Core.Web.Routes
{
    public interface IRouterPublisher
    {
        /// <summary>
        /// Registers the routes.
        /// </summary>
        /// <param name="routes">The routes.</param>
        void RegisterRoutes(RouteCollection routes);
    }
}
