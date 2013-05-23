using System;
using System.Collections.Generic;
using System.Linq;

namespace Aaron.Core.Infrastructure.DependencyManagement
{
    /// <summary>
    /// Registers service in the Aaron inversion of container upon start.
    /// </summary>
    public class DependencyAttributeRegistrator
    {
        private readonly ITypeFinder _finder;
        private readonly ICore _core;

        public DependencyAttributeRegistrator(ITypeFinder finder, ICore core)
        {
            this._finder = finder;
            this._core = core;
        }

        public virtual IEnumerable<AttributeInfo<DependencyAttribute>> FindServices()
        {
            foreach (Type type in _finder.FindClassesOfType<object>())
            {
                var attributes = type.GetCustomAttributes(typeof(DependencyAttribute), false);
                foreach (DependencyAttribute attribute in attributes)
                {
                    yield return new AttributeInfo<DependencyAttribute> { Attribute = attribute, DecoratedType = type };
                }
            }
        }

        public virtual void RegisterServices(IEnumerable<AttributeInfo<DependencyAttribute>> services)
        {
            foreach (var info in services)
            {
                info.Attribute.RegisterService(info, _core.ContainerManager);
            }
        }

        public virtual IEnumerable<AttributeInfo<DependencyAttribute>> FilterServices(IEnumerable<AttributeInfo<DependencyAttribute>> services, params string[] configurationKeys)
        {
            return services.Where(s => s.Attribute.Configuration == null || configurationKeys.Contains(s.Attribute.Configuration));
        }
    }
}
