using System.ComponentModel.DataAnnotations;
using Aaron.Core.Web;

namespace Aaron.Admin.Models.Common
{
    public class SEOModel : BaseEntityModel
    {
        public string MetaTitle { get; set; }

        [DataType(DataType.MultilineText)]
        public string MetaKeywords { get; set; }

        [DataType(DataType.MultilineText)]
        public string MetaDescription { get; set; }

        public string SEOUrlName { get; set; }
    }
}