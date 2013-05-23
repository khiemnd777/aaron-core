using System.Data.Entity.ModelConfiguration;
using Aaron.Core;
using Aaron.Core.Domain.Messages;

namespace Aaron.Core.Mapping.Messages
{
    public partial class MessageTemplateMap : BaseEntityTypeConfiguration<MessageTemplate, int>
    {
        public MessageTemplateMap()
            : base()
        {
            this.Property(mt => mt.Name).IsRequired().HasMaxLength(200);
            this.Property(mt => mt.BccEmailAddresses).HasMaxLength(200);
            this.Property(mt => mt.Subject).HasMaxLength(1000);
            this.Property(mt => mt.Body).IsMaxLength();
            this.Property(mt => mt.EmailAccountId).IsRequired();
        }
    }
}