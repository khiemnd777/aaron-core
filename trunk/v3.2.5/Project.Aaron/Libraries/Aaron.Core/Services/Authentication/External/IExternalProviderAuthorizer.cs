//Contributor:  Nicholas Mayne


namespace Aaron.Core.Services.Authentication.External
{
    public partial interface IExternalProviderAuthorizer
    {
        AuthorizeState Authorize(string returnUrl);
    }
}