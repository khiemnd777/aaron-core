using Aaron.Core;
using Aaron.Core.Domain.Logging;

namespace Aaron.Data.Mapping.Logging
{
    public partial class ActivityLogTypeMap : BaseEntityTypeConfiguration<ActivityLogType, int>
    {
        public ActivityLogTypeMap()
            : base()
        {
            this.Property(alt => alt.SystemKeyword).IsRequired().HasMaxLength(100);
            this.Property(alt => alt.Name).IsRequired().HasMaxLength(200);
        }
    }
}
