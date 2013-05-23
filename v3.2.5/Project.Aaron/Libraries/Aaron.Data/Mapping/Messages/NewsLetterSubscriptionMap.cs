using System;
using Aaron.Core;
using Aaron.Core.Domain.Messages;

namespace Aaron.Data.Mapping.Messages
{
    public partial class NewsLetterSubscriptionMap : BaseEntityTypeConfiguration<NewsLetterSubscription, int>
    {
        public NewsLetterSubscriptionMap()
            : base()
        {
            this.Property(n => n.Email).IsRequired().HasMaxLength(255);
        }
    }
}
