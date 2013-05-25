using System.Collections.Generic;
using Aaron.Core.Web;

namespace $rootnamespace$.Models.Common
{
    public partial class LanguageSelectorModel : BaseModel
    {
        public LanguageSelectorModel()
        {
            AvailableLanguages = new List<LanguageModel>();
        }

        public IList<LanguageModel> AvailableLanguages { get; set; }

        public int CurrentLanguageId { get; set; }

        public bool UseImages { get; set; }
    }
}