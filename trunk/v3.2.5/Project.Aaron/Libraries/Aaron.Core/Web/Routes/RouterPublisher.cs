using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Aaron.Core.Infrastructure;

namespace Aaron.Core.Web.Routes
{
    public class RouterPublisher : IRouterPublisher
    {
        private readonly ITypeFinder _typeFinder;

        public RouterPublisher(ITypeFinder typeFinder)
        {
            _typeFinder = typeFinder;
        }

        /// <summary>
        /// Registers the routes.
        /// </summary>
        /// <param name="routes">The routes.</param>
        public void RegisterRoutes(RouteCollection routes)
        {
            var routerProviderTypes = _typeFinder.FindClassesOfType<IRouterProvider>();
            var routerProviderList = new List<IRouterProvider>();
            foreach (var routerProvider in routerProviderTypes)
            {
                var _r = Activator.CreateInstance(routerProvider) as IRouterProvider;
                routerProviderList.Add(_r);
            }
            routerProviderList = routerProviderList.OrderByDescending(o => o.Order).ToList();
            routerProviderList.ForEach(x => x.RegisterRoutes(routes));
        }
    }
}
