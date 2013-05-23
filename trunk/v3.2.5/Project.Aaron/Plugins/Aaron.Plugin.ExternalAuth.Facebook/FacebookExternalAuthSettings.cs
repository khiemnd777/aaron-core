using Aaron.Core.SysConfiguration;

namespace Aaron.Plugin.ExternalAuth.Facebook
{
    public class FacebookExternalAuthSettings : ISettings
    {
        public string ClientKeyIdentifier { get; set; }
        public string ClientSecret { get; set; }
    }
}
