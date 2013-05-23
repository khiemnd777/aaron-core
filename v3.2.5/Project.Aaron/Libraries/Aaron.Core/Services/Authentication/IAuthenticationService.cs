using Aaron.Core.Domain.Accounts;

namespace Aaron.Core.Services.Authentication
{
    /// <summary>
    /// Authentication service interface
    /// </summary>
    public partial interface IAuthenticationService
    {
        void SignIn(Account customer, bool createPersistentCookie);
        void SignOut();
        Account GetAuthenticatedAccount();
    }
}