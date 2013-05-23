//Contributor:  Nicholas Mayne

using Facebook;
using Aaron.Core.Domain.Accounts;
using Aaron.Core.Services.Authentication.External;

namespace Aaron.Plugin.ExternalAuth.Facebook.Core
{
    public interface IOAuthProviderFacebookAuthorizer : IExternalProviderAuthorizer
    {
        FacebookClient GetClient(Account account);
    }
}