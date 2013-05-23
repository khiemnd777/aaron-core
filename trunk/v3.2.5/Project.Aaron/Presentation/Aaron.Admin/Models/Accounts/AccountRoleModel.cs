using Aaron.Core.Web;
using System.ComponentModel.DataAnnotations;

namespace Aaron.Admin.Models.Accounts
{
    public class AccountRoleModel : BaseEntityModel
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name of role.
        /// </value>
        [Display(Name="Vai trò")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="AccountRoleModel" /> is active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if active is true, the role will be used; otherwise, <c>false</c>.
        /// </value>
        [Display(Name="Kích hoạt?")]
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is system role.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is system role, the role couldn't be deleted; otherwise, <c>false</c>.
        /// </value>
        public bool IsSystemRole { get; set; }

        /// <summary>
        /// Gets or sets the name of the system.
        /// </summary>
        /// <value>
        /// The name of the system role.
        /// </value>
        [Display(Name = "Tên hệ thống")]
        public string SystemName { get; set; }
    }
}