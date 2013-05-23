using System;
using System.Collections.Generic;
using Aaron.Core.Domain.Accounts;

namespace Aaron.Core.Services.Accounts
{
    /// <summary>
    /// Account content service interface
    /// </summary>
    public partial interface IAccountContentService
    {
        /// <summary>
        /// Deletes a Account content
        /// </summary>
        /// <param name="content">Account content</param>
        void DeleteAccountContent(AccountContent content);

        /// <summary>
        /// Gets all Account content
        /// </summary>
        /// <param name="AccountId">Account identifier; 0 to load all records</param>
        /// <param name="approved">A value indicating whether to content is approved; null to load all records</param>
        /// <returns>Account content</returns>
        IList<AccountContent> GetAllAccountContent(int AccountId, bool? approved);

        /// <summary>
        /// Gets all Account content
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="AccountId">Account identifier; 0 to load all records</param>
        /// <param name="approved">A value indicating whether to content is approved; null to load all records</param>
        /// <param name="fromUtc">Item creation from; null to load all records</param>
        /// <param name="toUtc">Item item creation to; null to load all records</param>
        /// <returns>Account content</returns>
        IList<T> GetAllAccountContent<T>(int AccountId, bool? approved,
            DateTime? fromUtc = null, DateTime? toUtc = null) where T : AccountContent;

        /// <summary>
        /// Gets a Account content
        /// </summary>
        /// <param name="contentId">Account content identifier</param>
        /// <returns>Account content</returns>
        AccountContent GetAccountContentById(int contentId);

        /// <summary>
        /// Inserts a Account content
        /// </summary>
        /// <param name="content">Account content</param>
        void InsertAccountContent(AccountContent content);

        /// <summary>
        /// Updates a Account content
        /// </summary>
        /// <param name="content">Account content</param>
        void UpdateAccountContent(AccountContent content);
    }
}
