using System;
using System.Web;
using System.Reflection;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Text;
using System.Web.Routing;
using System.IO;
using System.Linq;
using Autofac;
using Autofac.Core;
using Autofac.Integration.Mvc;
using Autofac.Builder;
using Aaron.Data;
using Aaron.Core;
using Aaron.Core.Fakes;
using Aaron.Core.Infrastructure;
using Aaron.Core.Infrastructure.Config;
using Aaron.Core.Infrastructure.DependencyManagement;
using Aaron.Core.SysConfiguration;
using Aaron.Core.Data;
using Aaron.Core.Caching;
using Aaron.Core.Threading;
using Aaron.Core.Services.Configuration;
using Aaron.Core.Services.Tasks;
using Aaron.Core.Services.Messages;
using Aaron.Core.Services.Accounts;
using Aaron.Core.Services.Authentication;
using Aaron.Core.Services.Security;
using Aaron.Core.Services.Utilities;
using Aaron.Core.Services.Localization;
using Aaron.Core.Services.Cms;
using Aaron.Core.Services.Logging;
using Aaron.Core.Services.Authentication.External;
using Aaron.Core.Services.Helpers;
using Aaron.Core.Services.Directory;
using Aaron.Core.Web;
using Aaron.Core.Web.Routes;
using Aaron.Core.Web.UI.Media;
using Aaron.Core.Web.UI.GenericCatalog;
using Aaron.Core.Web.UI.Analysis;
using Aaron.Core.Web.Themes;
using Aaron.Core.Web.UI;
using Aaron.Core.Security;
using Aaron.Core.Installation;
using Aaron.Core.Plugins;
using Aaron.Core.Web.EmbeddedViews;

namespace Aaron.Registrar
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            //HTTP context and other related stuff
            builder.Register(c =>
                //register FakeHttpContext when HttpContext is not available
                HttpContext.Current != null ?
                (new HttpContextWrapper(HttpContext.Current) as HttpContextBase) :
                (new FakeHttpContext("~/") as HttpContextBase))
                .As<HttpContextBase>()
                .InstancePerHttpRequest();
            builder.Register(c => c.Resolve<HttpContextBase>().Request)
                .As<HttpRequestBase>()
                .InstancePerHttpRequest();
            builder.Register(c => c.Resolve<HttpContextBase>().Response)
                .As<HttpResponseBase>()
                .InstancePerHttpRequest();
            builder.Register(c => c.Resolve<HttpContextBase>().Server)
                .As<HttpServerUtilityBase>()
                .InstancePerHttpRequest();
            builder.Register(c => c.Resolve<HttpContextBase>().Session)
                .As<HttpSessionStateBase>()
                .InstancePerHttpRequest();

            // webhelper
            builder.RegisterType<WebHelper>().As<IWebHelper>().InstancePerHttpRequest();
            builder.RegisterType<WebCurrentActivity>().As<ICurrentActivity>().InstancePerHttpRequest();
            // controllers
            builder.RegisterControllers(typeFinder.GetAssemblies().ToArray());

            var dataSettingsManager = new DataSettingsManager();
            var dataProviderSettings = dataSettingsManager.LoadSettings();
            builder.Register(c => dataSettingsManager.LoadSettings()).As<DataSettings>();
            builder.Register(x => new BaseDataProviderManager(x.Resolve<DataSettings>())).As<DataProviderManager>().InstancePerDependency();


            builder.Register(x => (IEfDataProvider)x.Resolve<DataProviderManager>().LoadDataProvider()).As<IDataProvider>().InstancePerDependency();
            builder.Register(x => (IEfDataProvider)x.Resolve<DataProviderManager>().LoadDataProvider()).As<IEfDataProvider>().InstancePerDependency();

            if (dataProviderSettings != null && dataProviderSettings.IsValid())
            {
                var efDataProviderManager = new BaseDataProviderManager(dataSettingsManager.LoadSettings());
                var dataProvider = (IEfDataProvider)efDataProviderManager.LoadDataProvider();
                dataProvider.InitConnectionFactory();

                builder.Register<IContext>(c => new AaronDbContext(dataProviderSettings)).InstancePerHttpRequest();
            }
            else
                builder.Register<IContext>(c => new AaronDbContext(dataSettingsManager.LoadSettings())).InstancePerHttpRequest();

            // bind IEmbeddedViewResolver
            builder.RegisterType<EmbeddedViewResolver>().As<IEmbeddedViewResolver>().SingleInstance();

            // bind IRouterPublisher
            builder.RegisterType<RouterPublisher>().As<IRouterPublisher>().SingleInstance();

            // cache manager
            builder.RegisterType<MemoryCacheManager>().As<ICacheManager>().Named<ICacheManager>("cache_static").SingleInstance();
            builder.RegisterType<PerRequestCacheManager>().As<ICacheManager>().Named<ICacheManager>("cache_per_request").InstancePerHttpRequest();

            builder.RegisterGeneric(typeof(ImplRepository<>)).As(typeof(IRepository<>)).InstancePerHttpRequest();

            // settings
            builder.RegisterType<SettingService>().As<ISettingService>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("cache_static"))
                .InstancePerHttpRequest();

            // message binding
            builder.RegisterType<EmailSender>().As<IEmailSender>().InstancePerHttpRequest();
            builder.RegisterType<EmailAccountService>().As<IEmailAccountService>().InstancePerHttpRequest();
            builder.RegisterType<QueuedEmailService>().As<IQueuedEmailService>().InstancePerHttpRequest();
            builder.RegisterType<Tokenizer>().As<ITokenizer>().InstancePerHttpRequest();
            builder.RegisterType<NewsLetterSubscriptionService>().As<INewsLetterSubscriptionService>().InstancePerHttpRequest();

            // account binding
            builder.RegisterType<AccountContentService>().As<IAccountContentService>().InstancePerHttpRequest();
            builder.RegisterType<AccountRegistrationService>().As<IAccountRegistrationService>().InstancePerHttpRequest();
            builder.RegisterType<AccountService>().As<IAccountService>().InstancePerHttpRequest();
            builder.RegisterType<LocalizationService>().As<ILocalizationService>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("cache_static"))
                .InstancePerHttpRequest();

            builder.RegisterType<LocalizedEntityService>().As<ILocalizedEntityService>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("cache_static"))
                .InstancePerHttpRequest();

            builder.RegisterType<LanguageService>().As<ILanguageService>().InstancePerHttpRequest();

            // authentication binding
            builder.RegisterType<FormsAuthenticationService>().As<IAuthenticationService>().InstancePerHttpRequest();

            // security binding
            builder.RegisterType<EncryptionService>().As<IEncryptionService>().InstancePerHttpRequest();
            builder.RegisterType<PermissionService>().As<IPermissionService>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("cache_static"))
                .InstancePerHttpRequest();
            builder.RegisterType<StandardPermissionProvider>().As<IPermissionProvider>().InstancePerHttpRequest();
            builder.RegisterType<PermissionProviderSetup>().As<IPermissionProviderSetup>().InstancePerHttpRequest();

            // Utilities binding
            builder.RegisterType<GenericAttributeService>().As<IGenericAttributeService>().InstancePerHttpRequest();

            // External Authentication
            builder.RegisterType<OpenAuthenticationService>().As<IOpenAuthenticationService>().InstancePerHttpRequest();
            builder.RegisterType<ExternalAuthorizer>().As<IExternalAuthorizer>().InstancePerHttpRequest();

            // Plugins
            builder.RegisterType<PluginFinder>().As<IPluginFinder>().InstancePerHttpRequest();

            // Widgets
            builder.RegisterType<WidgetService>().As<IWidgetService>().InstancePerHttpRequest();

            // thread manager
            builder.RegisterType<ThreadManager>().As<IThreadManager>().SingleInstance();

            // Schedule Task
            builder.RegisterType<ScheduleTaskService>().As<IScheduleTaskService>().InstancePerHttpRequest();

            // Logger
            builder.RegisterType<DefaultLogger>().As<ILogger>().InstancePerHttpRequest();

            // system configuration
            builder.RegisterGeneric(typeof(SysConfigurationProvider<>)).As(typeof(ISysConfigurationProvider<>));
            builder.RegisterSource(new SettingsSource());

            // services
            builder.RegisterSource(new ServicesSource());

            // installation binding do not put with-in InSingletonScope() because itself is;
            builder.RegisterType<InstallationPublisher>().As<IInstallationPublisher>().InstancePerHttpRequest();

            builder.RegisterType<PageTitleBuilder>().As<IPageTitleBuilder>().InstancePerHttpRequest();
            builder.RegisterType<DateTimeHelper>().As<IDateTimeHelper>().InstancePerHttpRequest();
            builder.RegisterType<ThemeContext>().As<IThemeContext>().InstancePerHttpRequest();
            builder.RegisterType<ThemeProvider>().As<IThemeProvider>().InstancePerHttpRequest();
            builder.RegisterType<GeoCountryLookup>().As<IGeoCountryLookup>().InstancePerHttpRequest();
            builder.RegisterType<MediaTagBuilder>().As<IMediaTagBuilder>().InstancePerHttpRequest();
            builder.RegisterType<GenericCatalogBuilder>().As<IGenericCatalogBuilder>().InstancePerHttpRequest();
            builder.RegisterType<AnalysisNSocialNetworkBuilder>().As<IAnalysisNSocialNetworkBuilder>().InstancePerHttpRequest();
            builder.Register<UrlHelper>(helper =>
                new UrlHelper(new RequestContext
                    (helper.Resolve<HttpContextBase>(),
                    RouteTable.Routes
                    .GetRouteData(helper.Resolve<HttpContextBase>()))))
                    .InstancePerHttpRequest();
            builder.RegisterType<MessageTokenProvider>().As<IMessageTokenProvider>().InstancePerHttpRequest();
        }

        public int Order
        {
            get { return 0; }
        }
    }

    public class ServicesSource : IRegistrationSource
    {

        private static readonly MethodInfo method = typeof(ServicesSource)
            .GetMethod("RegistrySource", 
            BindingFlags.NonPublic | 
            BindingFlags.Static);

        private static IComponentRegistration RegistrySource<TService>() where TService : IServices
        {
            Type t = null;
            return RegistrationBuilder.ForDelegate((c, p) =>
            {
                // Type Finder
                var typeFinder = c.Resolve<ITypeFinder>();
                if (typeFinder.FindClassesOfType<TService>().First() != null)
                {
                    t = typeFinder.FindClassesOfType<TService>().First();
                }
                var _pobj = t.GetConstructors()
                    .SelectMany(x => x.GetParameters())
                    .Select(x => 
                    {
                        return c.Resolve(x.ParameterType);
                    });

                return (TService)Activator.CreateInstance(t, _pobj.ToArray());
            })
            .As<TService>()
            .InstancePerHttpRequest()
            .CreateRegistration();
        }

        public bool IsAdapterForIndividualComponents
        {
            get { return false; }
        }

        public IEnumerable<IComponentRegistration> RegistrationsFor(Service service,
            Func<Service, IEnumerable<IComponentRegistration>> registrationAccessor)
        {
            var ts = service as TypedService;
            if (ts != null && typeof(IServices).IsAssignableFrom(ts.ServiceType) && ts.ServiceType.IsInterface)
            {
                var builder = method.MakeGenericMethod(ts.ServiceType);
                yield return (IComponentRegistration)builder.Invoke(null, null);
            }
        }
    }

    public class SettingsSource : IRegistrationSource
    {
        static readonly MethodInfo BuildMethod = typeof(SettingsSource).GetMethod(
            "BuildRegistration",
            BindingFlags.Static | BindingFlags.NonPublic);

        public IEnumerable<IComponentRegistration> RegistrationsFor(
                Service service,
                Func<Service, IEnumerable<IComponentRegistration>> registrations)
        {

            var ts = service as TypedService;
            if (ts != null && typeof(ISettings).IsAssignableFrom(ts.ServiceType))
            {
                var buildMethod = BuildMethod.MakeGenericMethod(ts.ServiceType);
                yield return (IComponentRegistration)buildMethod.Invoke(null, null);
            }
        }

        static IComponentRegistration BuildRegistration<TSettings>() where TSettings : ISettings, new()
        {
            return RegistrationBuilder
                .ForDelegate((c, p) => c.Resolve<ISysConfigurationProvider<TSettings>>().Settings)
                .InstancePerHttpRequest()
                .CreateRegistration();
        }

        public bool IsAdapterForIndividualComponents { get { return false; } }
    }
}
