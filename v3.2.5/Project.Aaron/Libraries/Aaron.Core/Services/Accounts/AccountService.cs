using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aaron.Core.Data;
using Aaron.Core.Caching;
using Aaron.Core.Domain.Accounts;
using Aaron.Core.Domain.Utilities;
using System.Globalization;
using System.Diagnostics;

namespace Aaron.Core.Services.Accounts
{
    public partial class  AccountService : IAccountService
    {
        private const string ACCOUNTROLES_ALL_KEY = "Aaron.accountrole.all-{0}";
        private const string ACCOUNTROLES_BY_ID_KEY = "Aaron.accountrole.id-{0}";
        private const string ACCOUNTROLES_BY_SYSTEMNAME_KEY = "Aaron.accountrole.systemname-{0}";
        private const string ACCOUNTROLES_PATTERN_KEY = "Aaron.accountrole.";

        private readonly IRepository<Account> _accountRepository;
        private readonly IRepository<AccountRole> _accountRoleRepository;
        private readonly IRepository<GenericAttribute> _gaRepository;
        private readonly ICacheManager _cacheManager;
        //private readonly IEventPublisher _eventPublisher;

        public AccountService(ICacheManager cacheManager,
            IRepository<Account> AccountRepository,
            IRepository<AccountRole> AccountRoleRepository,
            IRepository<GenericAttribute> gaRepository/*,
            IEventPublisher eventPublisher*/)
        {
            this._cacheManager = cacheManager;
            this._accountRepository = AccountRepository;
            this._accountRoleRepository = AccountRoleRepository;
            this._gaRepository = gaRepository;
            //this._eventPublisher = eventPublisher;
        }

        #region Accounts

        public IList<Account> GetAllAccounts(DateTime? registrationFrom,
           DateTime? registrationTo, int[] AccountRoleIds, string email, string username,
           string firstName, string lastName, int dayOfBirth, int monthOfBirth)
        {
            var query = _accountRepository.Table;
            if (registrationFrom.HasValue)
                query = query.Where(c => registrationFrom.Value <= c.CreatedOnUtc);
            if (registrationTo.HasValue)
                query = query.Where(c => registrationTo.Value >= c.CreatedOnUtc);
            query = query.Where(c => !c.Deleted);
            if (AccountRoleIds != null && AccountRoleIds.Length > 0)
                query = query.Where(c => c.AccountRoles.Select(cr => cr.Id).Intersect(AccountRoleIds).Count() > 0);
            if (!String.IsNullOrWhiteSpace(email))
                query = query.Where(c => c.Email.Contains(email));
            if (!String.IsNullOrWhiteSpace(username))
                query = query.Where(c => c.Username.Contains(username));
            if (!String.IsNullOrWhiteSpace(firstName))
            {
                query = query
                    .Join(_gaRepository.Table, x => x.Id, y => y.EntityId, (x, y) => new { Account = x, Attribute = y })
                    .Where((z => z.Attribute.KeyGroup == "Account" &&
                        z.Attribute.Key == SystemAttributeNames.FirstName &&
                        z.Attribute.Value.Contains(firstName)))
                    .Select(z => z.Account);
            }
            if (!String.IsNullOrWhiteSpace(lastName))
            {
                query = query
                    .Join(_gaRepository.Table, x => x.Id, y => y.EntityId, (x, y) => new { Account = x, Attribute = y })
                    .Where((z => z.Attribute.KeyGroup == "Account" &&
                        z.Attribute.Key == SystemAttributeNames.LastName &&
                        z.Attribute.Value.Contains(lastName)))
                    .Select(z => z.Account);
            }
            //date of birth is stored as a string into database.
            //we also know that date of birth is stored in the following format YYYY-MM-DD (for example, 1983-02-18).
            //so let's search it as a string
            if (dayOfBirth > 0 && monthOfBirth > 0)
            {
                //both are specified
                string dateOfBirthStr = monthOfBirth.ToString("00", CultureInfo.InvariantCulture) + "-" + dayOfBirth.ToString("00", CultureInfo.InvariantCulture);
                //EndsWith is not supported by SQL Server Compact
                //so let's use the following workaround http://social.msdn.microsoft.com/Forums/is/sqlce/thread/0f810be1-2132-4c59-b9ae-8f7013c0cc00

                //we also cannot use Length function in SQL Server Compact (not supported in this context)
                //z.Attribute.Value.Length - dateOfBirthStr.Length = 5
                //dateOfBirthStr.Length = 5
                query = query
                    .Join(_gaRepository.Table, x => x.Id, y => y.EntityId, (x, y) => new { Account = x, Attribute = y })
                    .Where((z => z.Attribute.KeyGroup == "Account" &&
                        z.Attribute.Key == SystemAttributeNames.DateOfBirth &&
                        z.Attribute.Value.Substring(5, 5) == dateOfBirthStr))
                    .Select(z => z.Account);
            }
            else if (dayOfBirth > 0)
            {
                //only day is specified
                string dateOfBirthStr = dayOfBirth.ToString("00", CultureInfo.InvariantCulture);
                //EndsWith is not supported by SQL Server Compact
                //so let's use the following workaround http://social.msdn.microsoft.com/Forums/is/sqlce/thread/0f810be1-2132-4c59-b9ae-8f7013c0cc00

                //we also cannot use Length function in SQL Server Compact (not supported in this context)
                //z.Attribute.Value.Length - dateOfBirthStr.Length = 8
                //dateOfBirthStr.Length = 2
                query = query
                    .Join(_gaRepository.Table, x => x.Id, y => y.EntityId, (x, y) => new { Account = x, Attribute = y })
                    .Where((z => z.Attribute.KeyGroup == "Account" &&
                        z.Attribute.Key == SystemAttributeNames.DateOfBirth &&
                        z.Attribute.Value.Substring(8, 2) == dateOfBirthStr))
                    .Select(z => z.Account);
            }
            else if (monthOfBirth > 0)
            {
                //only month is specified
                string dateOfBirthStr = "-" + monthOfBirth.ToString("00", CultureInfo.InvariantCulture) + "-";
                query = query
                    .Join(_gaRepository.Table, x => x.Id, y => y.EntityId, (x, y) => new { Account = x, Attribute = y })
                    .Where((z => z.Attribute.KeyGroup == "Account" &&
                        z.Attribute.Key == SystemAttributeNames.DateOfBirth &&
                        z.Attribute.Value.Contains(dateOfBirthStr)))
                    .Select(z => z.Account);
            }

            query = query.OrderByDescending(c => c.CreatedOnUtc);
            return query.ToList();
        }

        /// <summary>
        /// Gets all Accounts
        /// </summary>
        /// <param name="registrationFrom">Account registration from; null to load all Accounts</param>
        /// <param name="registrationTo">Account registration to; null to load all Accounts</param>
        /// <param name="AccountRoleIds">A list of Account role identifiers to filter by (at least one match); pass null or empty list in order to load all Accounts; </param>
        /// <param name="email">Email; null to load all Accounts</param>
        /// <param name="username">Username; null to load all Accounts</param>
        /// <param name="firstName">First name; null to load all Accounts</param>
        /// <param name="lastName">Last name; null to load all Accounts</param>
        /// <param name="dayOfBirth">Day of birth; 0 to load all Accounts</param>
        /// <param name="monthOfBirth">Month of birth; 0 to load all Accounts</param>
        /// <param name="company">Company; null to load all Accounts</param>
        /// <param name="phone">Phone; null to load all Accounts</param>
        /// <param name="zipPostalCode">Phone; null to load all Accounts</param>
        /// <param name="loadOnlyWithShoppingCart">Value indicating whther to load Accounts only with shopping cart</param>
        /// <param name="sct">Value indicating what shopping cart type to filter; userd when 'loadOnlyWithShoppingCart' param is 'true'</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Account collection</returns>
        public virtual IPagedList<Account> GetAllAccounts(DateTime? registrationFrom,
            DateTime? registrationTo, int[] AccountRoleIds, string email, string username,
            string firstName, string lastName, int dayOfBirth, int monthOfBirth, int pageIndex, int pageSize)
        {
            var query = _accountRepository.Table;
            if (registrationFrom.HasValue)
                query = query.Where(c => registrationFrom.Value <= c.CreatedOnUtc);
            if (registrationTo.HasValue)
                query = query.Where(c => registrationTo.Value >= c.CreatedOnUtc);
            query = query.Where(c => !c.Deleted);
            if (AccountRoleIds != null && AccountRoleIds.Length > 0)
                query = query.Where(c => c.AccountRoles.Select(cr => cr.Id).Intersect(AccountRoleIds).Count() > 0);
            if (!String.IsNullOrWhiteSpace(email))
                query = query.Where(c => c.Email.Contains(email));
            if (!String.IsNullOrWhiteSpace(username))
                query = query.Where(c => c.Username.Contains(username));
            if (!String.IsNullOrWhiteSpace(firstName))
            {
                query = query
                    .Join(_gaRepository.Table, x => x.Id, y => y.EntityId, (x, y) => new { Account = x, Attribute = y })
                    .Where((z => z.Attribute.KeyGroup == "Account" &&
                        z.Attribute.Key == SystemAttributeNames.FirstName &&
                        z.Attribute.Value.Contains(firstName)))
                    .Select(z => z.Account);
            }
            if (!String.IsNullOrWhiteSpace(lastName))
            {
                query = query
                    .Join(_gaRepository.Table, x => x.Id, y => y.EntityId, (x, y) => new { Account = x, Attribute = y })
                    .Where((z => z.Attribute.KeyGroup == "Account" &&
                        z.Attribute.Key == SystemAttributeNames.LastName &&
                        z.Attribute.Value.Contains(lastName)))
                    .Select(z => z.Account);
            }
            //date of birth is stored as a string into database.
            //we also know that date of birth is stored in the following format YYYY-MM-DD (for example, 1983-02-18).
            //so let's search it as a string
            if (dayOfBirth > 0 && monthOfBirth > 0)
            {
                //both are specified
                string dateOfBirthStr = monthOfBirth.ToString("00", CultureInfo.InvariantCulture) + "-" + dayOfBirth.ToString("00", CultureInfo.InvariantCulture);
                //EndsWith is not supported by SQL Server Compact
                //so let's use the following workaround http://social.msdn.microsoft.com/Forums/is/sqlce/thread/0f810be1-2132-4c59-b9ae-8f7013c0cc00

                //we also cannot use Length function in SQL Server Compact (not supported in this context)
                //z.Attribute.Value.Length - dateOfBirthStr.Length = 5
                //dateOfBirthStr.Length = 5
                query = query
                    .Join(_gaRepository.Table, x => x.Id, y => y.EntityId, (x, y) => new { Account = x, Attribute = y })
                    .Where((z => z.Attribute.KeyGroup == "Account" &&
                        z.Attribute.Key == SystemAttributeNames.DateOfBirth &&
                        z.Attribute.Value.Substring(5, 5) == dateOfBirthStr))
                    .Select(z => z.Account);
            }
            else if (dayOfBirth > 0)
            {
                //only day is specified
                string dateOfBirthStr = dayOfBirth.ToString("00", CultureInfo.InvariantCulture);
                //EndsWith is not supported by SQL Server Compact
                //so let's use the following workaround http://social.msdn.microsoft.com/Forums/is/sqlce/thread/0f810be1-2132-4c59-b9ae-8f7013c0cc00

                //we also cannot use Length function in SQL Server Compact (not supported in this context)
                //z.Attribute.Value.Length - dateOfBirthStr.Length = 8
                //dateOfBirthStr.Length = 2
                query = query
                    .Join(_gaRepository.Table, x => x.Id, y => y.EntityId, (x, y) => new { Account = x, Attribute = y })
                    .Where((z => z.Attribute.KeyGroup == "Account" &&
                        z.Attribute.Key == SystemAttributeNames.DateOfBirth &&
                        z.Attribute.Value.Substring(8, 2) == dateOfBirthStr))
                    .Select(z => z.Account);
            }
            else if (monthOfBirth > 0)
            {
                //only month is specified
                string dateOfBirthStr = "-" + monthOfBirth.ToString("00", CultureInfo.InvariantCulture) + "-";
                query = query
                    .Join(_gaRepository.Table, x => x.Id, y => y.EntityId, (x, y) => new { Account = x, Attribute = y })
                    .Where((z => z.Attribute.KeyGroup == "Account" &&
                        z.Attribute.Key == SystemAttributeNames.DateOfBirth &&
                        z.Attribute.Value.Contains(dateOfBirthStr)))
                    .Select(z => z.Account);
            }
            
            query = query.OrderByDescending(c => c.CreatedOnUtc);

            var accounts = new PagedList<Account>(query, pageIndex, pageSize);
            return accounts;
        }

        /// <summary>
        /// Gets all Accounts by Account format (including deleted ones)
        /// </summary>
        /// <param name="passwordFormat">Password format</param>
        /// <returns>Accounts</returns>
        public virtual IList<Account> GetAllAccountsByPasswordFormat(PasswordFormat passwordFormat)
        {
            int passwordFormatId = (int)passwordFormat;

            var query = _accountRepository.Table;
            query = query.Where(c => c.PasswordFormatId == passwordFormatId);
            query = query.OrderByDescending(c => c.CreatedOnUtc);
            var Accounts = query.ToList();
            return Accounts;
        }

        /// <summary>
        /// Gets online Accounts
        /// </summary>
        /// <param name="lastActivityFromUtc">Account last activity date (from)</param>
        /// <param name="AccountRoleIds">A list of Account role identifiers to filter by (at least one match); pass null or empty list in order to load all Accounts; </param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Account collection</returns>
        public virtual IPagedList<Account> GetOnlineAccounts(DateTime lastActivityFromUtc,
            int[] AccountRoleIds, int pageIndex, int pageSize)
        {
            var query = _accountRepository.Table;
            query = query.Where(c => lastActivityFromUtc <= c.LastActivityDateUtc);
            query = query.Where(c => !c.Deleted);
            if (AccountRoleIds != null && AccountRoleIds.Length > 0)
                query = query.Where(c => c.AccountRoles.Select(cr => cr.Id).Intersect(AccountRoleIds).Count() > 0);

            query = query.OrderByDescending(c => c.LastActivityDateUtc);
            var Accounts = new PagedList<Account>(query, pageIndex, pageSize);
            return Accounts;
        }

        /// <summary>
        /// Gets all Accounts by Account role id
        /// </summary>
        /// <param name="AccountRoleId">Account role identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Account collection</returns>
        public virtual IList<Account> GetAccountsByAccountRoleId(int AccountRoleId, bool showHidden = false)
        {
            var query = from c in _accountRepository.Table
                        from cr in c.AccountRoles
                        where (showHidden || c.Active) &&
                            !c.Deleted &&
                            cr.Id == AccountRoleId
                        orderby c.CreatedOnUtc descending
                        select c;

            var Accounts = query.ToList();
            return Accounts;
        }

        /// <summary>
        /// Delete a Account
        /// </summary>
        /// <param name="Account">Account</param>
        public virtual void DeleteAccount(Account Account)
        {
            if (Account == null)
                throw new ArgumentNullException("Account");

            if (Account.IsSystemAccount)
                throw new AaronException(string.Format("System Account account ({0}) could not be deleted", Account.SystemName));

            Account.Deleted = true;
            UpdateAccount(Account);
        }

        /// <summary>
        /// Gets a Account
        /// </summary>
        /// <param name="AccountId">Account identifier</param>
        /// <returns>A Account</returns>
        public virtual Account GetAccountById(int accountId)
        {
            if (accountId == 0)
                return null;

            var account = _accountRepository.GetById(accountId);
            return account;
        }

        /// <summary>
        /// Get Accounts by identifiers
        /// </summary>
        /// <param name="AccountIds">Account identifiers</param>
        /// <returns>Accounts</returns>
        public virtual IList<Account> GetAccountsByIds(int[] accountIds)
        {
            if (accountIds == null || accountIds.Length == 0)
                return new List<Account>();

            var query = from c in _accountRepository.Table
                        where accountIds.Contains(c.Id)
                        select c;
            var accounts = query.ToList();
            //sort by passed identifiers
            var sortedAccounts = new List<Account>();
            foreach (int id in accountIds)
            {
                var account = accounts.Find(x => x.Id == id);
                if (account != null)
                    sortedAccounts.Add(account);
            }
            return sortedAccounts;
        }

        /// <summary>
        /// Gets a Account by GUID
        /// </summary>
        /// <param name="AccountGuid">Account GUID</param>
        /// <returns>A Account</returns>
        public virtual Account GetAccountByGuid(Guid accountGuid)
        {
            if (accountGuid == Guid.Empty)
                return null;

            var query = from c in _accountRepository.Table
                        where c.AccountGuid == accountGuid
                        orderby c.Id
                        select c;
            var account = query.FirstOrDefault();
            return account;
        }

        /// <summary>
        /// Get Account by email
        /// </summary>
        /// <param name="email">Email</param>
        /// <returns>Account</returns>
        public virtual Account GetAccountByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            var query = from c in _accountRepository.Table
                        orderby c.Id
                        where c.Email == email
                        select c;
            var account = query.FirstOrDefault();
            return account;
        }

        /// <summary>
        /// Get Account by system name
        /// </summary>
        /// <param name="systemName">System name</param>
        /// <returns>Account</returns>
        public virtual Account GetAccountBySystemName(string systemName)
        {
            if (string.IsNullOrWhiteSpace(systemName))
                return null;

            var query = from c in _accountRepository.Table
                        orderby c.Id
                        where c.SystemName == systemName
                        select c;
            var account = query.FirstOrDefault();
            return account;
        }

        /// <summary>
        /// Get Account by username
        /// </summary>
        /// <param name="username">Username</param>
        /// <returns>Account</returns>
        public virtual Account GetAccountByUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return null;

            var query = from c in _accountRepository.Table
                        orderby c.Id
                        where c.Username == username
                        select c;
            var account = query.FirstOrDefault();
            return account;
        }

        /// <summary>
        /// Get Accounts by language identifier
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Accounts</returns>
        public virtual IList<Account> GetAccountsByLanguageId(int languageId)
        {
            var query = _accountRepository.Table;
            if (languageId > 0)
                query = query.Where(c => c.LanguageId.HasValue && c.LanguageId.Value == languageId);
            else
                query = query.Where(c => !c.LanguageId.HasValue);
            query = query.OrderBy(c => c.Id);
            var accounts = query.ToList();
            return accounts;
        }
        
        /// <summary>
        /// Insert a guest Account
        /// </summary>
        /// <returns>Account</returns>
        public virtual Account InsertGuestAccount()
        {
            var account = new Account()
            {
                AccountGuid = Guid.NewGuid(),
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
            };

            //add to 'Guests' role
            var guestRole = GetAccountRoleBySystemName(SystemAccountRoleNames.Guests);
            if (guestRole == null)
                throw new AaronException("'Guests' role could not be loaded");
            account.AccountRoles.Add(guestRole);

            _accountRepository.Insert(account);

            return account;
        }

        /// <summary>
        /// Insert a Account
        /// </summary>
        /// <param name="Account">Account</param>
        public virtual void InsertAccount(Account account)
        {
            if (account == null)
                throw new ArgumentNullException("account");

            _accountRepository.Insert(account);

            //event notification
            //_eventPublisher.EntityInserted(account);
        }

        /// <summary>
        /// Updates the Account
        /// </summary>
        /// <param name="Account">Account</param>
        public virtual void UpdateAccount(Account account)
        {
            if (account == null)
                throw new ArgumentNullException("account");

            _accountRepository.Update(account);

            //event notification
            //_eventPublisher.EntityUpdated(account);
        }

        /// <summary>
        /// Delete guest Account records
        /// </summary>
        /// <param name="registrationFrom">Account registration from; null to load all Accounts</param>
        /// <param name="registrationTo">Account registration to; null to load all Accounts</param>
        /// <param name="onlyWithoutShoppingCart">A value indicating whether to delete Accounts only without shopping cart</param>
        /// <returns>Number of deleted Accounts</returns>
        public virtual int DeleteGuestAccounts(DateTime? registrationFrom,
            DateTime? registrationTo)
        {
            var guestRole = GetAccountRoleBySystemName(SystemAccountRoleNames.Guests);
            if (guestRole == null)
                throw new AaronException("'Guests' role could not be loaded");

            var query = _accountRepository.Table;
            if (registrationFrom.HasValue)
                query = query.Where(c => registrationFrom.Value <= c.CreatedOnUtc);
            if (registrationTo.HasValue)
                query = query.Where(c => registrationTo.Value >= c.CreatedOnUtc);
            query = query.Where(c => c.AccountRoles.Select(cr => cr.Id).Contains(guestRole.Id));
            
            //no Account content
            query = query.Where(c => c.AccountContent.Count() == 0);
            //don't delete system accounts
            query = query.Where(c => !c.IsSystemAccount);
            var accounts = query.ToList();

            int numberOfDeletedAccounts = 0;
            foreach (var c in accounts)
            {
                try
                {
                    //delete from database
                    _accountRepository.Delete(c);
                    numberOfDeletedAccounts++;
                }
                catch (Exception exc)
                {
                    Debug.WriteLine(exc);
                }
            }
            return numberOfDeletedAccounts;
        }

        #endregion

        #region Account roles

        /// <summary>
        /// Delete a Account role
        /// </summary>
        /// <param name="AccountRole">Account role</param>
        public virtual void DeleteAccountRole(AccountRole accountRole)
        {
            if (accountRole == null)
                throw new ArgumentNullException("AccountRole");

            if (accountRole.IsSystemRole)
                throw new AaronException("System role could not be deleted");

            _accountRoleRepository.Delete(accountRole);

            _cacheManager.RemoveByPattern(ACCOUNTROLES_PATTERN_KEY);

            //event notification
            //_eventPublisher.EntityDeleted(accountRole);
        }

        /// <summary>
        /// Gets a Account role
        /// </summary>
        /// <param name="AccountRoleId">Account role identifier</param>
        /// <returns>Account role</returns>
        public virtual AccountRole GetAccountRoleById(int accountRoleId)
        {
            if (accountRoleId == 0)
                return null;

            string key = string.Format(ACCOUNTROLES_BY_ID_KEY, accountRoleId);
            return _cacheManager.Get(key, () =>
            {
                var AccountRole = _accountRoleRepository.GetById(accountRoleId);
                return AccountRole;
            });
        }

        /// <summary>
        /// Gets a Account role
        /// </summary>
        /// <param name="systemName">Account role system name</param>
        /// <returns>Account role</returns>
        public virtual AccountRole GetAccountRoleBySystemName(string systemName)
        {
            if (String.IsNullOrWhiteSpace(systemName))
                return null;

            string key = string.Format(ACCOUNTROLES_BY_SYSTEMNAME_KEY, systemName);
            return _cacheManager.Get(key, () =>
            {
                var query = from cr in _accountRoleRepository.Table
                            orderby cr.Id
                            where cr.SystemName == systemName
                            select cr;
                var AccountRole = query.FirstOrDefault();
                return AccountRole;
            });
        }

        /// <summary>
        /// Gets all Account roles
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Account role collection</returns>
        public virtual IList<AccountRole> GetAllAccountRoles(bool showHidden = false)
        {
            string key = string.Format(ACCOUNTROLES_ALL_KEY, showHidden);
            return _cacheManager.Get(key, () =>
            {
                var query = from cr in _accountRoleRepository.Table
                            orderby cr.Name
                            where (showHidden || cr.Active)
                            select cr;
                var accountRoles = query.ToList();
                return accountRoles;
            });
        }

        /// <summary>
        /// Inserts a Account role
        /// </summary>
        /// <param name="accountRole">Account role</param>
        public virtual void InsertAccountRole(AccountRole accountRole)
        {
            if (accountRole == null)
                throw new ArgumentNullException("accountRole");

            _accountRoleRepository.Insert(accountRole);

            _cacheManager.RemoveByPattern(ACCOUNTROLES_PATTERN_KEY);

            //event notification
            //_eventPublisher.EntityInserted(accountRole);
        }

        /// <summary>
        /// Updates the Account role
        /// </summary>
        /// <param name="AccountRole">Account role</param>
        public virtual void UpdateAccountRole(AccountRole accountRole)
        {
            if (accountRole == null)
                throw new ArgumentNullException("accountRole");

            _accountRoleRepository.Update(accountRole);

            _cacheManager.RemoveByPattern(ACCOUNTROLES_PATTERN_KEY);

            //event notification
            //_eventPublisher.EntityUpdated(accountRole);
        }

        #endregion
    }
}
