using System.Collections.Generic;
using Aaron.Admin.Models.Accounts;
using Aaron.Core.Web.Mvc;
using Aaron.Core.Web;

namespace Aaron.Admin.Models.Security
{
    public partial class PermissionMappingModel : BaseModel
    {
        public PermissionMappingModel()
        {
            AvailablePermissions = new List<PermissionRecordModel>();
            AvailableAccountRoles = new List<AccountRoleModel>();
            Allowed = new Dictionary<string, IDictionary<int, bool>>();
        }
        public IList<PermissionRecordModel> AvailablePermissions { get; set; }
        public IList<AccountRoleModel> AvailableAccountRoles { get; set; }

        //[permission system name] / [customer role id] / [allowed]
        public IDictionary<string, IDictionary<int, bool>> Allowed { get; set; }
    }
}