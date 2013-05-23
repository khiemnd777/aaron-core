using System.ComponentModel.DataAnnotations;
using Aaron.Core.Web;
using Aaron.Core.Web.Mvc;

namespace Aaron.Plugin.ExternalAuth.Facebook.Models
{
    public class ConfigurationModel : BaseModel
    {
        [Display(Name="Client Key Identifier")]
        public string ClientKeyIdentifier { get; set; }

        [Display(Name = "Client Secret")]
        public string ClientSecret { get; set; }
    }
}