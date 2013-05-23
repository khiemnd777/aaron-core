using System;
using System.Collections.Generic;
using System.Linq;
using Aaron.Core;
using Aaron.Core.Caching;
using Aaron.Core.Data;
using Aaron.Core.Domain.Accounts;
using Aaron.Core.Domain.Security;
using Aaron.Core.Services.Accounts;

namespace Aaron.Core.Services.Security
{
    /// <summary>
    /// Permission service
    /// </summary>
    public partial class PermissionService : IPermissionService
    {
        #region Constants
        /// <summary>
        /// Cache key for storing a valie indicating whether a certain account role has a permission
        /// </summary>
        /// <remarks>
        /// {0} : account role id
        /// {1} : permission system name
        /// </remarks>
        private const string PERMISSIONS_ALLOWED_KEY = "Aaron.permission.allowed-{0}-{1}";
        private const string PERMISSIONS_PATTERN_KEY = "Aaron.permission.";
        #endregion

        #region Fields

        private readonly IRepository<PermissionRecord> _permissionRecordRepository;
        private readonly IAccountService _accountService;
        private readonly ICurrentActivity _currentActivity;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="permissionPecordRepository">Permission repository</param>
        /// <param name="accountService">Account service</param>
        /// <param name="currentActivity">Current activity</param>
        /// <param name="cacheManager">Cache manager</param>
        public PermissionService(IRepository<PermissionRecord> permissionRecordRepository,
            IAccountService accountService,
            ICurrentActivity currentActivity,
            ICacheManager staticCacheManager)
        {
            this._permissionRecordRepository = permissionRecordRepository;
            this._accountService = accountService;
            this._currentActivity = currentActivity;
            this._cacheManager = staticCacheManager;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permissionRecordSystemName">Permission record system name</param>
        /// <param name="accountRole">Account role</param>
        /// <returns>true - authorized; otherwise, false</returns>
        protected virtual bool Authorize(string permissionRecordSystemName, AccountRole accountRole)
        {
            if (String.IsNullOrEmpty(permissionRecordSystemName))
                return false;

            string key = string.Format(PERMISSIONS_ALLOWED_KEY, accountRole.Id, permissionRecordSystemName);
            return _cacheManager.Get(key, () =>
            {
                foreach (var permission1 in accountRole.PermissionRecords)
                    if (permission1.SystemName.Equals(permissionRecordSystemName, StringComparison.InvariantCultureIgnoreCase))
                        return true;

                return false;
            });
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete a permission
        /// </summary>
        /// <param name="permission">Permission</param>
        public virtual void DeletePermissionRecord(PermissionRecord permission)
        {
            if (permission == null)
                throw new ArgumentNullException("permission");

            _permissionRecordRepository.Delete(permission);

            _cacheManager.RemoveByPattern(PERMISSIONS_PATTERN_KEY);
        }

        /// <summary>
        /// Gets a permission
        /// </summary>
        /// <param name="permissionId">Permission identifier</param>
        /// <returns>Permission</returns>
        public virtual PermissionRecord GetPermissionRecordById(int permissionId)
        {
            if (permissionId == 0)
                return null;

            return _permissionRecordRepository.GetById(permissionId);
        }

        /// <summary>
        /// Gets a permission
        /// </summary>
        /// <param name="systemName">Permission system name</param>
        /// <returns>Permission</returns>
        public virtual PermissionRecord GetPermissionRecordBySystemName(string systemName)
        {
            if (String.IsNullOrWhiteSpace(systemName))
                return null;

            var query = from pr in _permissionRecordRepository.Table
                        where pr.SystemName == systemName
                        orderby pr.Id
                        select pr;

            var permissionRecord = query.FirstOrDefault();
            return permissionRecord;
        }

        /// <summary>
        /// Gets all permissions
        /// </summary>
        /// <returns>Permissions</returns>
        public virtual IList<PermissionRecord> GetAllPermissionRecords()
        {
            var query = from pr in _permissionRecordRepository.Table
                        orderby pr.Name
                        select pr;
            var permissions = query.ToList();
            return permissions;
        }

        /// <summary>
        /// Inserts a permission
        /// </summary>
        /// <param name="permission">Permission</param>
        public virtual void InsertPermissionRecord(PermissionRecord permission)
        {
            if (permission == null)
                throw new ArgumentNullException("permission");

            _permissionRecordRepository.Insert(permission);

            _cacheManager.RemoveByPattern(PERMISSIONS_PATTERN_KEY);
        }

        /// <summary>
        /// Updates the permission
        /// </summary>
        /// <param name="permission">Permission</param>
        public virtual void UpdatePermissionRecord(PermissionRecord permission)
        {
            if (permission == null)
                throw new ArgumentNullException("permission");

            _permissionRecordRepository.Update(permission);

            _cacheManager.RemoveByPattern(PERMISSIONS_PATTERN_KEY);
        }

        /// <summary>
        /// Install permissions
        /// </summary>
        /// <param name="permissionProvider">Permission provider</param>
        public virtual void InstallPermissions(IPermissionProvider permissionProvider)
        {
            //install new permissions
            var permissions = permissionProvider.GetPermissions();
            foreach (var permission in permissions)
            {
                var permission1 = GetPermissionRecordBySystemName(permission.SystemName);
                if (permission1 == null)
                {
                    //new permission (install it)
                    permission1 = new PermissionRecord()
                    {
                        Name = permission.Name,
                        SystemName = permission.SystemName,
                        Category = permission.Category,
                    };


                    //default account role mappings
                    var defaultPermissions = permissionProvider.GetDefaultPermissions();
                    foreach (var defaultPermission in defaultPermissions)
                    {
                        var accountRole = _accountService.GetAccountRoleBySystemName(defaultPermission.AccountRoleSystemName);
                        if (accountRole == null)
                        {
                            //new role (save it)
                            accountRole = new AccountRole()
                            {
                                Name = defaultPermission.AccountRoleSystemName,
                                Active = true,
                                SystemName = defaultPermission.AccountRoleSystemName
                            };
                            _accountService.InsertAccountRole(accountRole);
                        }


                        var defaultMappingProvided = (from p in defaultPermission.PermissionRecords
                                                      where p.SystemName == permission1.SystemName
                                                      select p).Any();
                        var mappingExists = (from p in accountRole.PermissionRecords
                                             where p.SystemName == permission1.SystemName
                                             select p).Any();
                        if (defaultMappingProvided && !mappingExists)
                        {
                            permission1.AccountRoles.Add(accountRole);
                        }
                    }

                    //save new permission
                    InsertPermissionRecord(permission1);
                }
            }
        }

        /// <summary>
        /// Uninstall permissions
        /// </summary>
        /// <param name="permissionProvider">Permission provider</param>
        public virtual void UninstallPermissions(IPermissionProvider permissionProvider)
        {
            var permissions = permissionProvider.GetPermissions();
            foreach (var permission in permissions)
            {
                var permission1 = GetPermissionRecordBySystemName(permission.SystemName);
                if (permission1 != null)
                {
                    DeletePermissionRecord(permission1);
                }
            }
        }

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permission">Permission record</param>
        /// <returns>true - authorized; otherwise, false</returns>
        public virtual bool Authorize(PermissionRecord permission)
        {
            return Authorize(permission, _currentActivity.CurrentAccount);
        }

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permission">Permission record</param>
        /// <param name="account">Account</param>
        /// <returns>true - authorized; otherwise, false</returns>
        public virtual bool Authorize(PermissionRecord permission, Account account)
        {
            if (permission == null)
                return false;

            if (account == null)
                return false;

            //old implementation of Authorize method
            //var accountRoles = account.AccountRoles.Where(cr => cr.Active);
            //foreach (var role in accountRoles)
            //    foreach (var permission1 in role.PermissionRecords)
            //        if (permission1.SystemName.Equals(permission.SystemName, StringComparison.InvariantCultureIgnoreCase))
            //            return true;

            //return false;

            return Authorize(permission.SystemName, account);
        }

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permissionRecordSystemName">Permission record system name</param>
        /// <returns>true - authorized; otherwise, false</returns>
        public virtual bool Authorize(string permissionRecordSystemName)
        {
            return Authorize(permissionRecordSystemName, _currentActivity.CurrentAccount);
        }

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permissionRecordSystemName">Permission record system name</param>
        /// <param name="account">Account</param>
        /// <returns>true - authorized; otherwise, false</returns>
        public virtual bool Authorize(string permissionRecordSystemName, Account account)
        {
            if (String.IsNullOrEmpty(permissionRecordSystemName))
                return false;

            var accountRoles = account.AccountRoles.Where(cr => cr.Active);
            foreach (var role in accountRoles)
                if (Authorize(permissionRecordSystemName, role))
                    //yes, we have such permission
                    return true;

            //no permission found
            return false;
        }

        #endregion
    }
}