//Contributor:  Nicholas Mayne


namespace Aaron.Core.Services.Authentication.External
{
    public partial interface IExternalAuthorizer
    {
        AuthorizationResult Authorize(OpenAuthenticationParameters parameters);
    }
}