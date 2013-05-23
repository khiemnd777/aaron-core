using Aaron.Core;
using Aaron.Core.Domain.Logging;

namespace Aaron.Data.Mapping.Logging
{
    public partial class ActivityLogMap : BaseEntityTypeConfiguration<ActivityLog, int>
    {
        public ActivityLogMap()
            : base()
        {
            this.Property(al => al.Comment).IsRequired();

            this.HasRequired(al => al.ActivityLogType)
                .WithMany()
                .HasForeignKey(al => al.ActivityLogTypeId);

            this.HasRequired(al => al.Account)
                .WithMany()
                .HasForeignKey(al => al.AccountId);
        }
    }
}
