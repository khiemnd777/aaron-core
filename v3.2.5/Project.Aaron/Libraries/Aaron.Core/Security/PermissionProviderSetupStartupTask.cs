using Aaron.Core.Infrastructure;

namespace Aaron.Core.Security
{
    public class PermissionProviderSetupStartupTask : IStartupTask
    {
        public void Execute()
        {
            var _permissionProviderSetup = IoC.Resolve<IPermissionProviderSetup>();
            _permissionProviderSetup.InstallPermissions(true);
        }

        public int Order
        {
            get { return 1; }
        }
    }
}
