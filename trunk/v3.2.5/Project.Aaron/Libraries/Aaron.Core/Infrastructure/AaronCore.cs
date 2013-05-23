using System;
using System.Collections.Generic;
using System.Linq;
using Aaron.Core.Data;
using Aaron.Core.Infrastructure.DependencyManagement;
using Autofac;

namespace Aaron.Core.Infrastructure
{
    public class AaronCore : ICore
    {
        private ContainerManager _containerManager;

        public AaronCore() 
            : this(new ContainerConfigurer(), EventBroker.Instance)
		{
		}

		public AaronCore(ContainerConfigurer configurer, EventBroker broker)
		{
            InitializeContainer(configurer, broker);
		}

        //public AaronCore()
        //{
        //    IoC.Initialize();
        //    _resolver = Singleton<IDependencyResolver>.Instance;
        //}

        private void RunStartupTasks()
        {
            var typeFinder = Resolve<ITypeFinder>();
            var startUpTaskTypes = typeFinder.FindClassesOfType<IStartupTask>();
            var startUpTasks = new List<IStartupTask>();
            foreach (var startUpTaskType in startUpTaskTypes)
                startUpTasks.Add((IStartupTask)Activator.CreateInstance(startUpTaskType));
            //sort
            startUpTasks = startUpTasks.AsQueryable().OrderBy(st => st.Order).ToList();
            foreach (var startUpTask in startUpTasks)
                startUpTask.Execute();
        }

        private void InitializeContainer(ContainerConfigurer configurer, EventBroker broker)
        {
            var builder = new ContainerBuilder();

            _containerManager = new ContainerManager(builder.Build(), builder);
            configurer.Configure(this, _containerManager, broker);
        }

        public void Initialize()
        {
            //InstallationManager.Install();
            if (DataHelper.DatabaseIsExisted())
            {
                RunStartupTasks();
            }
        }

        public T Resolve<T>() where T:class
        {
            return ContainerManager.Resolve<T>();
        }

        public ContainerManager ContainerManager
        {
            get { return _containerManager; }
        }

        public object Resolve(Type type)
        {
            return ContainerManager.Resolve(type);
        }

        public T[] ResolveAll<T>()
        {
            return ContainerManager.ResolveAll<T>();
        }
    }
}
