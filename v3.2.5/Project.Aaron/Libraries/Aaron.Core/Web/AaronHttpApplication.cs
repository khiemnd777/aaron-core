using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Aaron.Core.Infrastructure;
using Aaron.Core.Web.Routes;
using Aaron.Core.Data;
using Aaron.Core.Services.Tasks;
using Aaron.Core.Web.Themes;
using System.Reflection;
using Aaron.Core.Web.Mvc;
using Aaron.Core.Domain.Common;
using Aaron.Core.Services.Logging;
using FluentValidation.Mvc;
using Aaron.Core.Web.EmbeddedViews;
using System.Web.Hosting;

namespace Aaron.Core.Web
{
    public class AaronHttpApplication : HttpApplication
    {
        public AaronHttpApplication()
        {
            
        }

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("favicon.ico");
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{resource}.txt"); 
            
            var router = IoC.Resolve<IRouterPublisher>();
            router.RegisterRoutes(routes);

            var nameOfWebAssembly = string.Empty;
            var typeFinder = IoC.Resolve<ITypeFinder>();
            var assemblies = typeFinder.GetAssemblies().ToList();
            assemblies.ForEach(a => 
            {
                foreach (var t in a.GetTypes())
                    if (!"Aaron.Admin".Equals(a.GetName().Name) && ("AccountController".Equals(t.Name) || "HomeController".Equals(t.Name)))
                        nameOfWebAssembly = a.GetName().Name;
            });

            // default
            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                new[] { nameOfWebAssembly + ".Controllers" }
                );
        }

        public virtual void CustomApplication_Start(){}
        public virtual void CustomApplication_BeginRequest(object sender, EventArgs e){}
        public virtual void CustomApplication_EndRequest(object sender, EventArgs e){}
        public virtual void CustomApplication_AuthenticateRequest(object sender, EventArgs e){}

        protected void Application_Start()
        {
            //MiniProfilerEF.Initialize_EF42();

            AaronManager.Initialize(false);

            var databaseIsInstalled = DataHelper.DatabaseIsExisted();

            var dependencyResolver = new AaronDependencyResolver();
            DependencyResolver.SetResolver(dependencyResolver);

            ModelBinders.Binders.Add(typeof(BaseModel), new AaronModelBinder());

            if (databaseIsInstalled)
            {
                //remove all view engines
                System.Web.Mvc.ViewEngines.Engines.Clear();
                //except the themeable razor view engine we use
                System.Web.Mvc.ViewEngines.Engines.Add(new ThemeableRazorViewEngine());
            }

            //Add some functionality on top of the default ModelMetadataProvider
            ModelMetadataProviders.Current = new AaronMetadataProvider();

            AreaRegistration.RegisterAllAreas();
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            //fluent validation
            DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;
            ModelValidatorProviders.Providers.Add(new FluentValidationModelValidatorProvider(new AaronValidatorFactory()));

            //register virtual path provider for embedded views
            var embeddedViewResolver = IoC.Resolve<IEmbeddedViewResolver>();
            var embeddedProvider = new EmbeddedViewVirtualPathProvider(embeddedViewResolver.GetEmbeddedViews());
            HostingEnvironment.RegisterVirtualPathProvider(embeddedProvider);

            if (databaseIsInstalled)
            {
                TaskManager.Instance.Initialize();
                TaskManager.Instance.Start();
            }

            CustomApplication_Start();
        }

        private void EnsureDatabaseIsInstalled()
        {
            var _webHelper = IoC.Resolve<IWebHelper>();
            var installUrl = string.Format("{0}install", _webHelper.GetWebLocation());

            if (!_webHelper.IsStaticResource(this.Request) &&
                !DataHelper.DatabaseIsExisted() &&
                !_webHelper.GetThisPageUrl(false).StartsWith(installUrl, StringComparison.InvariantCultureIgnoreCase))

                this.Response.Redirect(installUrl);
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            EnsureDatabaseIsInstalled();

            CustomApplication_BeginRequest(sender, e);

            if (DataHelper.DatabaseIsExisted() && IoC.Resolve<WebInformationSettings>().UseMiniProfiler)
            {
                //MiniProfiler.Start();
            }
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            CustomApplication_EndRequest(sender, e);
            if (DataHelper.DatabaseIsExisted() && IoC.Resolve<WebInformationSettings>().UseMiniProfiler)
            {
                //MiniProfiler.Stop();
            }
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            CustomApplication_AuthenticateRequest(sender, e);
        }

        protected void Application_Error(Object sender, EventArgs e)
        {
            LogException(Server.GetLastError());
        }

        protected void LogException(Exception exc)
        {
            if (exc == null)
                return;

            if (!DataHelper.DatabaseIsExisted())
                return;

            //ignore 404 HTTP errors
            var httpException = exc as HttpException;
            if (httpException != null && httpException.GetHttpCode() == 404 &&
                !IoC.Resolve<CommonSettings>().Log404Errors)
                return;

            try
            {
                //log
                var logger = IoC.Resolve<ILogger>();
                var currentActivity = IoC.Resolve<ICurrentActivity>();
                logger.Error(exc.Message, exc, currentActivity.CurrentAccount);
            }
            catch (Exception)
            {
                //don't throw new exception if occurs
            }
        }
    }
}
