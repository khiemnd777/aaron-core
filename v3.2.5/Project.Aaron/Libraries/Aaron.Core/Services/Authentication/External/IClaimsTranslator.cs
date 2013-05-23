//Contributor:  Nicholas Mayne


namespace Aaron.Core.Services.Authentication.External
{
    public partial interface IClaimsTranslator<T>
    {
        UserClaims Translate(T response);
    }
}