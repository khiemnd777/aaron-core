﻿using System;

namespace Aaron.Core.Domain.Messages
{
    public partial class NewsLetterSubscription : BaseEntity<int>
    {
        /// <summary>
        /// Gets or sets the newsletter subscription GUID
        /// </summary>
        public virtual Guid NewsLetterSubscriptionGuid { get; set; }

        /// <summary>
        /// Gets or sets the subcriber email
        /// </summary>
        public virtual string Email { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether subscription is active
        /// </summary>
        public virtual bool Active { get; set; }

        /// <summary>
        /// Gets or sets the date and time when subscription was created
        /// </summary>
        public virtual DateTime CreatedOnUtc { get; set; }
    }
}
