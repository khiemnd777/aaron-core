using Aaron.Core;
using System.Data.Entity.ModelConfiguration;
using Aaron.Core.Domain.Messages;

namespace Aaron.Core.Mapping.Messages
{
    public partial class CampaignMap : BaseEntityTypeConfiguration<Campaign, int>
    {
        public CampaignMap()
            : base()
        {

            this.Property(ea => ea.Name).IsRequired();
            this.Property(ea => ea.Subject).IsRequired();
            this.Property(ea => ea.Body).IsRequired();
        }
    }
}