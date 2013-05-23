using Aaron.Core.Infrastructure;
using Aaron.Registrar;
using Aaron.Plugin.ExternalAuth.Facebook.Core;
using Aaron.Core.Infrastructure.DependencyManagement;
using Autofac;

namespace Aaron.Plugin.ExternalAuth.Facebook
{
    public class Registrar : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            builder.RegisterType<FacebookProviderAuthorizer>().As<IOAuthProviderFacebookAuthorizer>();
        }

        public int Order
        {
            get { return 10; }
        }
    }
}
