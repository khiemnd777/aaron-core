using Aaron.Core.SysConfiguration;
namespace Aaron.Core.Domain.Common
{
    public class AnalysisNSocialNetworkSettings : ISettings
    {
        public string GoogleAnalysis { get; set; }
        public string GoogleInitCode { get; set; }
        public string FacebookID { get; set; }
        public string FacebookInitCode { get; set; }
    }
}
