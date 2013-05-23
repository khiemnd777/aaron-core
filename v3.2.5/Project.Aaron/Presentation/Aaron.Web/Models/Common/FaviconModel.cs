using Aaron.Core.Web;

namespace Aaron.Web.Models.Common
{
    public class FaviconModel : BaseModel
    {
        public bool Uploaded { get; set; }
        public string FaviconUrl { get; set; }
    }
}