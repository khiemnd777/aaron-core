using System.Collections.Generic;
using Aaron.Core.Domain.Accounts;

namespace Aaron.Core.Domain.Security
{
    /// <summary>
    /// Represents a permission record
    /// </summary>
    public class PermissionRecord : BaseEntity<int>
    {
        private ICollection<AccountRole> _accountRoles;

        /// <summary>
        /// Gets or sets the permission name
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the permission system name
        /// </summary>
        public virtual string SystemName { get; set; }

        /// <summary>
        /// Gets or sets the permission category
        /// </summary>
        public virtual string Category { get; set; }

        /// <summary>
        /// Gets or sets discount usage history
        /// </summary>
        public virtual ICollection<AccountRole> AccountRoles
        {
            get { return _accountRoles ?? (_accountRoles = new List<AccountRole>()); }
            protected set { _accountRoles = value; }
        }
    }
}
