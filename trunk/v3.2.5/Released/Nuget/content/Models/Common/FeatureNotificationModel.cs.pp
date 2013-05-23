using Aaron.Core.Web;

namespace $rootnamespace$.Models.Common
{
    public enum FeatureNotificationType
    {
        Upgrade = 1,
        Construction
    }
    public class FeatureNotificationModel : BaseModel
    {
        public string BackUrl { get; set; }
        public string Name { get; set; }
        public FeatureNotificationType Type { get; set; }
    }
}