using System;
using Aaron.Core.Infrastructure;
using Aaron.Core.Services.Security;

namespace Aaron.Core.Security
{
    public class PermissionProviderSetup : IPermissionProviderSetup
    {
        private readonly ITypeFinder _typeFinder;
        private readonly IPermissionService _permissionService;

        public PermissionProviderSetup(ITypeFinder typeFinder,
            IPermissionService permissionService)
        {
            _typeFinder = typeFinder;
            _permissionService = permissionService;
        }

        public void InstallPermissions(bool loadStandardPermission = false)
        {
            var typeFinder = IoC.Resolve<ITypeFinder>();
            var permissionService = IoC.Resolve<IPermissionService>();

            var permissionProviders = _typeFinder.FindClassesOfType<IPermissionProvider>();
            if (permissionProviders != null)
                foreach (var providerType in permissionProviders)
                {
                    if (!loadStandardPermission)
                        if (typeof(StandardPermissionProvider).Equals(providerType))
                            continue;
                    dynamic provider = Activator.CreateInstance(providerType);
                    _permissionService.InstallPermissions(provider);
                }
        }
    }
}
