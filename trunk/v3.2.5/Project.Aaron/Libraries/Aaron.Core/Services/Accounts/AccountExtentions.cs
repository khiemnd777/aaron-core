using System;
using System.Linq;
using Aaron.Core;
using Aaron.Core.Domain.Accounts;
using Aaron.Core.Infrastructure;
using Aaron.Core.Services.Localization;
using Aaron.Core.Services.Utilities;
using Aaron.Core.Utility;

namespace Aaron.Core.Services.Accounts
{
    public static class AccountExtentions
    {
        public static string Personalization(this Account account)
        {
            return string.IsNullOrEmpty(account.GetFullName()) ? account.Email : account.GetFullName();
        }

        /// <summary>
        /// Gets a value indicating whether Account is in a certain Account role
        /// </summary>
        /// <param name="Account">Account</param>
        /// <param name="AccountRoleSystemName">Account role system name</param>
        /// <param name="onlyActiveAccountRoles">A value indicating whether we should look only in active Account roles</param>
        /// <returns>Result</returns>
        public static bool IsInAccountRole(this Account account,
            string accountRoleSystemName, bool onlyActiveAccountRoles = true)
        {
            if (account == null)
                throw new ArgumentNullException("account");

            if (String.IsNullOrEmpty(accountRoleSystemName))
                throw new ArgumentNullException("accountRoleSystemName");

            var result = account.AccountRoles
                .Where(cr => !onlyActiveAccountRoles || cr.Active)
                .Where(cr => cr.SystemName == accountRoleSystemName)
                .FirstOrDefault() != null;
            return result;
        }

        /// <summary>
        /// Gets a value indicating whether Account a search engine
        /// </summary>
        /// <param name="Account">Account</param>
        /// <returns>Result</returns>
        public static bool IsSearchEngineAccount(this Account account)
        {
            if (account == null)
                throw new ArgumentNullException("account");

            if (!account.IsSystemAccount || String.IsNullOrEmpty(account.SystemName))
                return false;

            var result = account.SystemName.Equals(SystemAccountNames.SearchEngine, StringComparison.InvariantCultureIgnoreCase);
            return result;
        }

        /// <summary>
        /// Gets a value indicating whether Account is administrator
        /// </summary>
        /// <param name="Account">Account</param>
        /// <param name="onlyActiveAccountRoles">A value indicating whether we should look only in active Account roles</param>
        /// <returns>Result</returns>
        public static bool IsAdmin(this Account account, bool onlyActiveAccountRoles = true)
        {
            return IsInAccountRole(account, SystemAccountRoleNames.Administrators, onlyActiveAccountRoles);
        }

        /// <summary>
        /// Gets a value indicating whether Account is registered
        /// </summary>
        /// <param name="Account">Account</param>
        /// <param name="onlyActiveAccountRoles">A value indicating whether we should look only in active Account roles</param>
        /// <returns>Result</returns>
        public static bool IsRegistered(this Account account, bool onlyActiveAccountRoles = true)
        {
            return IsInAccountRole(account, SystemAccountRoleNames.Registered, onlyActiveAccountRoles);
        }

        /// <summary>
        /// Gets a value indicating whether Account is guest
        /// </summary>
        /// <param name="Account">Account</param>
        /// <param name="onlyActiveAccountRoles">A value indicating whether we should look only in active Account roles</param>
        /// <returns>Result</returns>
        public static bool IsGuest(this Account account, bool onlyActiveAccountRoles = true)
        {
            return IsInAccountRole(account, SystemAccountRoleNames.Guests, onlyActiveAccountRoles);
        }

        public static string GetFullName(this Account account)
        {
            if (account == null)
                throw new ArgumentNullException("account");
            var firstName = account.GetAttribute<string>(SystemAttributeNames.FirstName);
            var lastName = account.GetAttribute<string>(SystemAttributeNames.LastName);

            string fullName = "";
            if (!String.IsNullOrWhiteSpace(firstName) && !String.IsNullOrWhiteSpace(lastName))
                fullName = string.Format("{0} {1}", firstName, lastName);
            else
            {
                if (!String.IsNullOrWhiteSpace(firstName))
                    fullName = firstName;

                if (!String.IsNullOrWhiteSpace(lastName))
                    fullName = lastName;
            }
            return fullName;
        }

        /// <summary>
        /// Formats the account name
        /// </summary>
        /// <param name="account">Source</param>
        /// <returns>Formatted text</returns>
        public static string FormatUserName(this Account account)
        {
            return FormatUserName(account, false);
        }

        /// <summary>
        /// Formats the account name
        /// </summary>
        /// <param name="account">Source</param>
        /// <param name="stripTooLong">Strip too long account name</param>
        /// <returns>Formatted text</returns>
        public static string FormatUserName(this Account account, bool stripTooLong)
        {
            if (account == null)
                return string.Empty;

            if (account.IsGuest())
            {
                return IoC.Resolve<ILocalizationService>().GetResource("Account.Guest");
            }

            string result = string.Empty;
            switch (IoC.Resolve<AccountSettings>().AccountNameFormat)
            {
                case AccountNameFormat.ShowEmails:
                    result = account.Email;
                    break;
                case AccountNameFormat.ShowFullNames:
                    result = account.GetFullName();
                    break;
                case AccountNameFormat.ShowUsernames:
                    result = account.Username;
                    break;
                default:
                    break;
            }

            if (stripTooLong)
            {
                int maxLength = 0; // EngineContext.Current.Resolve<AccountSettings>().FormatNameMaxLength;
                if (maxLength > 0 && result.Length > maxLength)
                {
                    result = result.Substring(0, maxLength);
                }
            }

            return result;
        }
    }
}
