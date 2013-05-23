namespace Aaron.Core.Security
{
    public interface IPermissionProviderSetup
    {
        void InstallPermissions(bool loadStandardPermission = false);
    }
}
