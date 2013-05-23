using System;
using Aaron.Core.Domain.Accounts;

namespace Aaron.Core.Domain.Logging
{
    /// <summary>
    /// Represents an activity log record
    /// </summary>
    public partial class ActivityLog : BaseEntity<int>
    {
        #region Properties

        /// <summary>
        /// Gets or sets the activity log type identifier
        /// </summary>
        public virtual int ActivityLogTypeId { get; set; }

        /// <summary>
        /// Gets or sets the account identifier
        /// </summary>
        public virtual int AccountId { get; set; }

        /// <summary>
        /// Gets or sets the activity comment
        /// </summary>
        public virtual string Comment { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public virtual DateTime CreatedOnUtc { get; set; }

        #endregion

        #region Navigation Properties

        /// <summary>
        /// Gets the activity log type
        /// </summary>
        public virtual ActivityLogType ActivityLogType { get; set; }

        /// <summary>
        /// Gets the account
        /// </summary>
        public virtual Account Account { get; set; }

        #endregion
    }
}
