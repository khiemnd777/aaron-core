
namespace Aaron.Core.Services.Accounts
{
    public interface IInstallAccountFirstService : IServices
    {
        void Install(string defaultUserEmail, string defaultUserPassword);
    }
}
