using Aaron.Core;
using Aaron.Core.Domain.Logging;

namespace Aaron.Data.Mapping.Logging
{
    public partial class LogMap : BaseEntityTypeConfiguration<Log, int>
    {
        public LogMap()
        {
            this.Property(l => l.ShortMessage).IsRequired();
            this.Property(l => l.FullMessage);
            this.Property(l => l.IpAddress).HasMaxLength(200);

            this.Ignore(l => l.LogLevel);

            this.HasOptional(l => l.Account)
                .WithMany()
                .HasForeignKey(l => l.AccountId)
            .WillCascadeOnDelete(true);

        }
    }
}