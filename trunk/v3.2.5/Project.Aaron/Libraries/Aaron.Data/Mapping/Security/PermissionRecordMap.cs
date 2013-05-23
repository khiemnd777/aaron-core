using Aaron.Core;
using Aaron.Core.Domain.Security;

namespace Aaron.Data.Mapping.Security
{
    public class PermissionRecordMap : BaseEntityTypeConfiguration<PermissionRecord, int>
    {
        public PermissionRecordMap()
            : base()
        {
            this.Property(pr => pr.Name).IsRequired();
            this.Property(pr => pr.SystemName).IsRequired().HasMaxLength(255);
            this.Property(pr => pr.Category).IsRequired().HasMaxLength(255);

            this.HasMany(pr => pr.AccountRoles)
                .WithMany(cr => cr.PermissionRecords)
                .Map(m => m.ToTable("PermissionRecord_Role"));
        }
    }
}
