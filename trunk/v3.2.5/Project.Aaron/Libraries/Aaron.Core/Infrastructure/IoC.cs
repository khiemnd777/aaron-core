using System;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using System.Runtime.CompilerServices;
using Aaron.Core.Infrastructure.DependencyManagement;

namespace Aaron.Core.Infrastructure
{
    /// <summary>
    /// Inversion of Control factory implementation.
    /// </summary>
    public static class IoC
    {
        private static ICore core { get { return AaronManager.Current; } }

        public static ContainerManager Container { get { return core.ContainerManager; } }

        #region Methods

        public static T ResolveUnregistered<T>() where T : class
        {
            return core.ContainerManager
                .ResolveUnregistered(typeof(T)) as T;
        }

        public static object Resolve(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            return core.Resolve(type);
        }

        public static T Resolve<T>() where T:class
        {
            return core.Resolve<T>();
        }

        public static IEnumerable<T> ResolveAll<T>()
        {
            return core.ResolveAll<T>() as IEnumerable<T>;
        }

        #endregion
    }
}