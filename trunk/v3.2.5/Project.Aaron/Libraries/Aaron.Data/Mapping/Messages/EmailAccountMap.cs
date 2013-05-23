using Aaron.Core;
using Aaron.Core.Domain.Messages;

namespace Aaron.Data.Mapping.Messages
{
    public partial class EmailAccountMap : BaseEntityTypeConfiguration<EmailAccount, int>
    {
        public EmailAccountMap()
            : base()
        {
            this.Property(ea => ea.Email).IsRequired().HasMaxLength(255);
            this.Property(ea => ea.DisplayName).HasMaxLength(255);
            this.Property(ea => ea.Host).IsRequired().HasMaxLength(255);
            this.Property(ea => ea.Username).IsRequired().HasMaxLength(255);
            this.Property(ea => ea.Password).IsRequired().HasMaxLength(255);

            this.Ignore(ea => ea.FriendlyName);
        }
    }
}
