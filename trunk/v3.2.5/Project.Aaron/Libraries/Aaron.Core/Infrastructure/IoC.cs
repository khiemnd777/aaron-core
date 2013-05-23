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

        /*public static void Register<T>(T instance)
        {
            if (instance == null)
                throw new ArgumentNullException("instance");

            _resolver.Register(instance); 
        }

        public static void Inject<T>(T existing)
        {
            if (existing == null)
                throw new ArgumentNullException("existing");

            _resolver.Inject(existing);
        }

        public static T TryResolve<T>() where T:class
        {
            return _resolver.TryResolve<T>();
        }

        public static object TryResolve(Type type)
        {
            return _resolver.TryResolve(type);
        }

        public static object ResolveUnRegistered(Type type)
        {
            var constructors = type.GetConstructors();
            foreach (var constructor in constructors)
            {
                try
                {
                    var parameters = constructor.GetParameters();
                    var parameterInstances = new List<object>();
                    foreach (var parameter in parameters)
                    {
                        var service = Resolve(parameter.ParameterType);
                        if (service == null) throw new AaronException("Unkown dependency");
                        parameterInstances.Add(service);
                    }
                    return Activator.CreateInstance(type, parameterInstances.ToArray());
                }
                catch (AaronException)
                {

                }
            }
            throw new AaronException("No contructor was found that had all the dependencies satisfied.");
        }

        public static T Resolve<T>(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            return _resolver.Resolve<T>(type);
        }

        public static T Resolve<T>(string name)
        {
            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            return _resolver.Resolve<T>(name);
        }
        */

        public static T ResolveUnregistered<T>() where T : class
        {
            return core.ContainerManager
                .ResolveUnregistered(typeof(T)) as T;
        }

        public static object Resolve(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            //return _resolver.Resolve(type);
            return core.Resolve(type);
        }

        //public static T Resolve<T>(Type type, string name)
        //{
        //    if (type == null)
        //        throw new ArgumentNullException("type");
        //    if (name == null)
        //        throw new ArgumentNullException("name");

        //    return _resolver.Resolve<T>(type, name);
        //}

        public static T Resolve<T>() where T:class
        {
            //return _resolver.Resolve<T>();
            return core.Resolve<T>();
        }

        public static IEnumerable<T> ResolveAll<T>()
        {
            //return _resolver.ResolveAll<T>();
            return core.ResolveAll<T>() as IEnumerable<T>;
        }

        #endregion
    }
}