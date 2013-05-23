using System;
using System.Collections.Generic;
using Aaron.Core;
using Aaron.Core.Domain.Localization;

namespace Aaron.Core.Domain.Accounts
{
    public partial class Account : BaseEntity<int>
    {
        private ICollection<ExternalAuthenticationRecord> _externalAuthenticationRecords;
        private ICollection<AccountContent> _accountContent;
        private ICollection<AccountRole> _accountRoles;

        public Account()
        {
            this.AccountGuid = Guid.NewGuid();
            this.PasswordFormat = PasswordFormat.Clear;
        }

        public virtual Guid AccountGuid { get; set; }
        public virtual string Username { get; set; }
        public virtual string Email { get; set; }
        public virtual string Password { get; set; }

        public virtual int PasswordFormatId { get; set; }
        public virtual PasswordFormat PasswordFormat
        {
            get { return (PasswordFormat)PasswordFormatId; }
            set { this.PasswordFormatId = (int)value; }
        }

        public virtual string PasswordSalt { get; set; }

        /// <summary>
        /// Gets or sets the language identifier
        /// </summary>
        public virtual int? LanguageId { get; set; }

        /// <summary>
        /// Gets or sets the time zone identifier
        /// </summary>
        public virtual string TimeZoneId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the account is active
        /// </summary>
        public virtual bool Active { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the account has been deleted
        /// </summary>
        public virtual bool Deleted { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the account account is system
        /// </summary>
        public virtual bool IsSystemAccount { get; set; }

        /// <summary>
        /// Gets or sets the account system name
        /// </summary>
        public virtual string SystemName { get; set; }

        /// <summary>
        /// Gets or sets the last IP address
        /// </summary>
        public virtual string LastIpAddress { get; set; }

        /// <summary>
        /// Gets or sets the date and time of entity creation
        /// </summary>
        public virtual DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time of last login
        /// </summary>
        public virtual DateTime? LastLoginDateUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time of last activity
        /// </summary>
        public virtual DateTime LastActivityDateUtc { get; set; }

        /// <summary>
        /// Gets or sets the language
        /// </summary>
        public virtual Language Language { get; set; }

        /// <summary>
        /// Gets or sets account generated content
        /// </summary>
        public virtual ICollection<AccountContent> AccountContent
        {
            get { return _accountContent ?? (_accountContent = new List<AccountContent>()); }
            protected set { _accountContent = value; }
        }

        /// <summary>
        /// Gets or sets the account roles
        /// </summary>
        public virtual ICollection<AccountRole> AccountRoles
        {
            get { return _accountRoles ?? (_accountRoles = new List<AccountRole>()); }
            protected set { _accountRoles = value; }
        }

        /// <summary>
        /// Gets or sets Account generated content
        /// </summary>
        public virtual ICollection<ExternalAuthenticationRecord> ExternalAuthenticationRecords
        {
            get { return _externalAuthenticationRecords ?? (_externalAuthenticationRecords = new List<ExternalAuthenticationRecord>()); }
            protected set { _externalAuthenticationRecords = value; }
        }
    }
}
