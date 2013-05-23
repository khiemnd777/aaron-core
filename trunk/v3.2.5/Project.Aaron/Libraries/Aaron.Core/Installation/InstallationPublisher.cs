using System;
using System.Collections.Generic;
using System.Linq;
using Aaron.Core.Infrastructure;
using Aaron.Core.Data;

namespace Aaron.Core.Installation
{
    public class InstallationPublisher : IInstallationPublisher
    {
        private readonly ITypeFinder _typeFinder;

        public InstallationPublisher(ITypeFinder typeFinder)
        {
            _typeFinder = typeFinder;
        }

        public void Install()
        {
            if (!DataHelper.DatabaseIsExisted() || !DataHelper.TableIsExisted())
            {
                var installationPublisherTypes = _typeFinder.FindClassesOfType<IInstallationProvider>();
                var installationPublisherList = new List<IInstallationProvider>();

                foreach (var type in installationPublisherTypes)
                {
                    var _t = IoC.Container.ResolveUnregistered(type) as IInstallationProvider;
                    installationPublisherList.Add(_t);
                }

                installationPublisherList = installationPublisherList.OrderBy(o => o.Order).ToList();
                installationPublisherList.ForEach(a => a.Install(_typeFinder));

                DataHelper.ResetCache();
            }
        }
    }
}
