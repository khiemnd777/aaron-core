using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Aaron.Core.Infrastructure;

namespace Aaron.Core.Web.Mvc
{
    public class AaronDependencyResolver : IDependencyResolver
    {
        public object GetService(Type serviceType)
        {
            return IoC.Container.ResolveOptional(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            var type = typeof(IEnumerable<>).MakeGenericType(serviceType);
            return (IEnumerable<object>) IoC.Resolve(type);
        }
    }
}
