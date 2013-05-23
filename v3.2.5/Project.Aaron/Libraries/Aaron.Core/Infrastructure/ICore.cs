using System;
using Aaron.Core.Infrastructure.DependencyManagement;

namespace Aaron.Core.Infrastructure
{
    public interface ICore
    {
        ContainerManager ContainerManager { get; }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Resolve
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <returns>Result</returns>
        T Resolve<T>() where T : class;

        object Resolve(Type type);

        T[] ResolveAll<T>();
    }
}