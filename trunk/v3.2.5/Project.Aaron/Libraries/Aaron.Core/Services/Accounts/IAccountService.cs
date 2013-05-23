using System;
using System.Collections.Generic;
using Aaron.Core;
using Aaron.Core.Domain.Accounts;

namespace Aaron.Core.Services.Accounts
{
    /// <summary>
    /// Account service interface
    /// </summary>
    public partial interface IAccountService
    {
        #region Accounts

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
        /// <returns>Account collection</returns>
        IList<Account> GetAllAccounts(DateTime? registrationFrom,
           DateTime? registrationTo, int[] AccountRoleIds, string email, string username,
           string firstName, string lastName, int dayOfBirth, int monthOfBirth);


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
        IPagedList<Account> GetAllAccounts(DateTime? registrationFrom,
           DateTime? registrationTo, int[] AccountRoleIds, string email, string username,
           string firstName, string lastName, int dayOfBirth, int monthOfBirth, int pageIndex, int pageSize);

        /// <summary>
        /// Gets all Accounts by Account format (including deleted ones)
        /// </summary>
        /// <param name="passwordFormat">Password format</param>
        /// <returns>Accounts</returns>
        IList<Account> GetAllAccountsByPasswordFormat(PasswordFormat passwordFormat);

        /// <summary>
        /// Gets online Accounts
        /// </summary>
        /// <param name="lastActivityFromUtc">Account last activity date (from)</param>
        /// <param name="AccountRoleIds">A list of Account role identifiers to filter by (at least one match); pass null or empty list in order to load all Accounts; </param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Account collection</returns>
        IPagedList<Account> GetOnlineAccounts(DateTime lastActivityFromUtc,
            int[] AccountRoleIds, int pageIndex, int pageSize);

        /// <summary>
        /// Gets all Accounts by Account role id
        /// </summary>
        /// <param name="AccountRoleId">Account role identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Account collection</returns>
        IList<Account> GetAccountsByAccountRoleId(int AccountRoleId, bool showHidden = false);

        /// <summary>
        /// Delete a Account
        /// </summary>
        /// <param name="Account">Account</param>
        void DeleteAccount(Account Account);

        /// <summary>
        /// Gets a Account
        /// </summary>
        /// <param name="AccountId">Account identifier</param>
        /// <returns>A Account</returns>
        Account GetAccountById(int AccountId);

        /// <summary>
        /// Get Accounts by identifiers
        /// </summary>
        /// <param name="AccountIds">Account identifiers</param>
        /// <returns>Accounts</returns>
        IList<Account> GetAccountsByIds(int[] AccountIds);

        /// <summary>
        /// Gets a Account by GUID
        /// </summary>
        /// <param name="AccountGuid">Account GUID</param>
        /// <returns>A Account</returns>
        Account GetAccountByGuid(Guid AccountGuid);

        /// <summary>
        /// Get Account by email
        /// </summary>
        /// <param name="email">Email</param>
        /// <returns>Account</returns>
        Account GetAccountByEmail(string email);

        /// <summary>
        /// Get Account by system role
        /// </summary>
        /// <param name="systemName">System name</param>
        /// <returns>Account</returns>
        Account GetAccountBySystemName(string systemName);

        /// <summary>
        /// Get Account by username
        /// </summary>
        /// <param name="username">Username</param>
        /// <returns>Account</returns>
        Account GetAccountByUsername(string username);

        /// <summary>
        /// Get Accounts by language identifier
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Accounts</returns>
        IList<Account> GetAccountsByLanguageId(int languageId);

        /// <summary>
        /// Insert a guest Account
        /// </summary>
        /// <returns>Account</returns>
        Account InsertGuestAccount();

        /// <summary>
        /// Insert a Account
        /// </summary>
        /// <param name="Account">Account</param>
        void InsertAccount(Account Account);

        /// <summary>
        /// Updates the Account
        /// </summary>
        /// <param name="Account">Account</param>
        void UpdateAccount(Account Account);

        /// <summary>
        /// Delete guest Account records
        /// </summary>
        /// <param name="registrationFrom">Account registration from; null to load all Accounts</param>
        /// <param name="registrationTo">Account registration to; null to load all Accounts</param>
        /// <param name="onlyWithoutShoppingCart">A value indicating whether to delete Accounts only without shopping cart</param>
        /// <returns>Number of deleted Accounts</returns>
        int DeleteGuestAccounts(DateTime? registrationFrom,
           DateTime? registrationTo);

        #endregion

        #region Account roles

        /// <summary>
        /// Delete a Account role
        /// </summary>
        /// <param name="AccountRole">Account role</param>
        void DeleteAccountRole(AccountRole AccountRole);

        /// <summary>
        /// Gets a Account role
        /// </summary>
        /// <param name="AccountRoleId">Account role identifier</param>
        /// <returns>Account role</returns>
        AccountRole GetAccountRoleById(int AccountRoleId);

        /// <summary>
        /// Gets a Account role
        /// </summary>
        /// <param name="systemName">Account role system name</param>
        /// <returns>Account role</returns>
        AccountRole GetAccountRoleBySystemName(string systemName);

        /// <summary>
        /// Gets all Account roles
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Account role collection</returns>
        IList<AccountRole> GetAllAccountRoles(bool showHidden = false);

        /// <summary>
        /// Inserts a Account role
        /// </summary>
        /// <param name="AccountRole">Account role</param>
        void InsertAccountRole(AccountRole AccountRole);

        /// <summary>
        /// Updates the Account role
        /// </summary>
        /// <param name="AccountRole">Account role</param>
        void UpdateAccountRole(AccountRole AccountRole);

        #endregion
    }
}