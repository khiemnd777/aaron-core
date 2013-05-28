using System.Collections.Generic;
using Aaron.Core.SysConfiguration;

namespace Aaron.Core.Domain.Common
{
    public class SeoSettings : ISettings
    {
        public string PageTitleSeparator { get; set; }
        public PageTitleSeoAdjustment PageTitleSeoAdjustment { get; set; }
        public string DefaultTitle { get; set; }
        public string DefaultMetaKeywords { get; set; }
        public string DefaultMetaDescription { get; set; }
        public bool CanonicalUrlsEnabled { get; set; }
        public bool AllowUnicodeCharsInUrls { get; set; }
        public bool ConvertNonWesternChars { get; set; }
        public List<string> ReservedUrlRecordSlugs { get; set; }
    }
}