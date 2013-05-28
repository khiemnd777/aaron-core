using System;
using System.Collections.Generic;
using System.Linq;
using Aaron.Core;
using Aaron.Core.Caching;
using Aaron.Core.Data;
using Aaron.Core.Domain.Accounts;
using Aaron.Core.Domain.Security;

namespace Aaron.Core.Services.Security
{
    /// <summary>
    /// ACL service
    /// </summary>
    public partial class AclService : IAclService
    {
        #region Constants

        private const string ACLRECORD_BY_ENTITYID_NAME_KEY = "Aaron.aclrecord.entityid-name-{0}-{1}";
        private const string ACLRECORD_PATTERN_KEY = "Aaron.aclrecord.";

        #endregion

        #region Fields

        private readonly IRepository<AclRecord> _aclRecordRepository;
        private readonly ICurrentActivity _currentActivity;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="currentActivity">Work context</param>
        /// <param name="aclRecordRepository">ACL record repository</param>
        public AclService(ICacheManager cacheManager, ICurrentActivity currentActivity,
            IRepository<AclRecord> aclRecordRepository)
        {
            this._cacheManager = cacheManager;
            this._currentActivity = currentActivity;
            this._aclRecordRepository = aclRecordRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes an ACL record
        /// </summary>
        /// <param name="aclRecord">ACL record</param>
        public virtual void DeleteAclRecord(AclRecord aclRecord)
        {
            if (aclRecord == null)
                throw new ArgumentNullException("aclRecord");

            _aclRecordRepository.Delete(aclRecord);

            //cache
            _cacheManager.RemoveByPattern(ACLRECORD_PATTERN_KEY);
        }

        /// <summary>
        /// Gets an ACL record
        /// </summary>
        /// <param name="aclRecordId">ACL record identifier</param>
        /// <returns>ACL record</returns>
        public virtual AclRecord GetAclRecordById(int aclRecordId)
        {
            if (aclRecordId == 0)
                return null;

            var aclRecord = _aclRecordRepository.GetById(aclRecordId);
            return aclRecord;
        }

        /// <summary>
        /// Gets ACL records
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>ACL records</returns>
        public virtual IList<AclRecord> GetAclRecords<T>(T entity) where T : BaseEntity<int>, IAclSupported
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            int entityId = entity.Id;
            string entityName = typeof(T).Name;

            var query = from ur in _aclRecordRepository.Table
                        where ur.EntityId == entityId &&
                        ur.EntityName == entityName
                        select ur;
            var aclRecords = query.ToList();
            return aclRecords;
        }


        /// <summary>
        /// Inserts an ACL record
        /// </summary>
        /// <param name="aclRecord">ACL record</param>
        public virtual void InsertAclRecord(AclRecord aclRecord)
        {
            if (aclRecord == null)
                throw new ArgumentNullException("aclRecord");

            _aclRecordRepository.Insert(aclRecord);

            //cache
            _cacheManager.RemoveByPattern(ACLRECORD_PATTERN_KEY);
        }

        /// <summary>
        /// Inserts an ACL record
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Entity</param>
        public virtual void InsertAclRecord<T>(T entity, int accountRoleId) where T : BaseEntity<int>, IAclSupported
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            if (accountRoleId == 0)
                throw new ArgumentOutOfRangeException("accountRoleId");

            int entityId = entity.Id;
            string entityName = typeof(T).Name;

            var aclRecord = new AclRecord()
            {
                EntityId = entityId,
                EntityName = entityName,
                AccountRoleId = accountRoleId
            };

            InsertAclRecord(aclRecord);
        }

        /// <summary>
        /// Updates the ACL record
        /// </summary>
        /// <param name="aclRecord">ACL record</param>
        public virtual void UpdateAclRecord(AclRecord aclRecord)
        {
            if (aclRecord == null)
                throw new ArgumentNullException("aclRecord");

            _aclRecordRepository.Update(aclRecord);

            //cache
            _cacheManager.RemoveByPattern(ACLRECORD_PATTERN_KEY);
        }

        /// <summary>
        /// Find account role identifiers with granted access
        /// </summary>
        /// <param name="entityId">Entity identifier</param>
        /// <param name="entityName">Entity name</param>
        /// <returns>Account role identifiers</returns>
        public virtual int[] GetAccountRoleIdsWithAccess<T>(T entity) where T : BaseEntity<int>, IAclSupported
        {
            if (entity == null)
                throw new ArgumentNullException("product");

            int entityId = entity.Id;
            string entityName = typeof(T).Name;

            string key = string.Format(ACLRECORD_BY_ENTITYID_NAME_KEY, entityId, entityName);
            return _cacheManager.Get(key, () =>
            {
                var query = from ur in _aclRecordRepository.Table
                            where ur.EntityId == entityId &&
                            ur.EntityName == entityName 
                            select ur.AccountRoleId;
                var result = query.ToArray();
                //little hack here. nulls aren't cacheable so set it to ""
                if (result == null)
                    result = new int[0];
                return result;
            });
        }

        /// <summary>
        /// Authorize ACL permission
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Wntity</param>
        /// <returns>true - authorized; otherwise, false</returns>
        public virtual bool Authorize<T>(T entity) where T : BaseEntity<int>, IAclSupported
        {
            return Authorize(entity, _currentActivity.CurrentAccount);
        }

        /// <summary>
        /// Authorize ACL permission
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Wntity</param>
        /// <param name="account">Account</param>
        /// <returns>true - authorized; otherwise, false</returns>
        public virtual bool Authorize<T>(T entity, Account account) where T : BaseEntity<int>, IAclSupported
        {
            if (entity == null)
                return false;

            if (account == null)
                return false;

            if (!entity.SubjectToAcl)
                return true;

            foreach (var role1 in account.AccountRoles.Where(cr => cr.Active))
                foreach (var role2Id in GetAccountRoleIdsWithAccess(entity))
                    if (role1.Id == role2Id)
                        //yes, we have such permission
                        return true;

            //no permission found
            return false;
        }
        #endregion
    }
}