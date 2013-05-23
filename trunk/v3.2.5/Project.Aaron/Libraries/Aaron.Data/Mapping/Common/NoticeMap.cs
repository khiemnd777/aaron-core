using Aaron.Core;
using Aaron.Core.Domain.Common;

namespace Aaron.Core.Mapping.Common
{
    public class NoticeMap : BaseEntityTypeConfiguration<Notice, int>
    {
        public NoticeMap()
            : base()
        {
            this.Property(n => n.Name).HasMaxLength(255);
            this.Property(n => n.Content)
                .IsMaxLength()
                .IsOptional();
            this.HasMany(n => n.Accounts)
                .WithMany()
                .Map(n => 
                {
                    n.MapLeftKey("NoticeId");
                    n.MapRightKey("AccountId");
                    n.ToTable("NoticeAccount");
                });
        }
    }
}
