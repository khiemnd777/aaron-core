using System;
using System.Linq;
using System.Web.Routing;
using System.Web.Mvc;
using Aaron.Core.Infrastructure;

namespace Aaron.Core.Web.Routes
{
    public static class RouteExtensions
    {
        public static void Default(this RouteCollection routes)
        {
            var nameOfWebAssembly = string.Empty;
            var typeFinder = IoC.Resolve<ITypeFinder>();
            var assemblies = typeFinder.GetAssemblies().ToList();
            assemblies.ForEach(a =>
            {
                a.GetTypes()
                    .ForEach(t => 
                    { 
                        if (!"Aaron.Admin".Equals(a.GetName().Name) && ("AccountController".Equals(t.Name) || "HomeController".Equals(t.Name)))
                        nameOfWebAssembly = a.GetName().Name;
                    });
            });

            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                new[] { nameOfWebAssembly + ".Controllers" }
                );
        }
    }
}