using System;
using System.Collections.Generic;
using System.Linq;
using Aaron.Core.Caching;
using Aaron.Core.Data;
using Aaron.Core.Domain.Accounts;
//using Aaron.Core.Services.Events;

namespace Aaron.Core.Services.Accounts
{
    /// <summary>
    /// Account content service
    /// </summary>
    public partial class AccountContentService : IAccountContentService
    {
        #region Fields

        private readonly IRepository<AccountContent> _contentRepository;
        //private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="contentRepository">Account content repository</param>
        /// <param name="eventPublisher">Event published</param>
        public AccountContentService(ICacheManager cacheManager,
            IRepository<AccountContent> contentRepository/*,
            IEventPublisher eventPublisher*/)
        {
            _cacheManager = cacheManager;
            _contentRepository = contentRepository;
            //_eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a account content
        /// </summary>
        /// <param name="content">Account content</param>
        public virtual void DeleteAccountContent(AccountContent content)
        {
            if (content == null)
                throw new ArgumentNullException("content");

            _contentRepository.Delete(content);

            //event notification
            //_eventPublisher.EntityDeleted(content);
        }

        /// <summary>
        /// Gets all Account content
        /// </summary>
        /// <param name="AccountId">Account identifier; 0 to load all records</param>
        /// <param name="approved">A value indicating whether to content is approved; null to load all records</param>
        /// <returns>Account content</returns>
        public virtual IList<AccountContent> GetAllAccountContent(int accountId, bool? approved)
        {
            var query = from c in _contentRepository.Table
                        orderby c.CreatedOnUtc
                        where !approved.HasValue || c.IsApproved == approved &&
                        (accountId == 0 || c.AccountId == accountId)
                        select c;
            var content = query.ToList();
            return content;
        }

        /// <summary>
        /// Gets all account content
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="accountId">Account identifier; 0 to load all records</param>
        /// <param name="approved">A value indicating whether to content is approved; null to load all records</param> 
        /// <param name="fromUtc">Item creation from; null to load all records</param>
        /// <param name="toUtc">Item item creation to; null to load all records</param>
        /// <returns>Account content</returns>
        public virtual IList<T> GetAllAccountContent<T>(int accountId, bool? approved,
            DateTime? fromUtc = null, DateTime? toUtc = null) where T : AccountContent
        {
            var query = _contentRepository.Table;
            if (approved.HasValue)
                query = query.Where(c => c.IsApproved == approved);
            if (accountId > 0)
                query = query.Where(c => c.AccountId == accountId);
            if (fromUtc.HasValue)
                query = query.Where(c => fromUtc.Value <= c.CreatedOnUtc);
            if (toUtc.HasValue)
                query = query.Where(c => toUtc.Value >= c.CreatedOnUtc);
            query = query.OrderBy(c => c.CreatedOnUtc);
            var content = query.OfType<T>().ToList();
            return content;
        }

        /// <summary>
        /// Gets a account content
        /// </summary>
        /// <param name="contentId">Account content identifier</param>
        /// <returns>Account content</returns>
        public virtual AccountContent GetAccountContentById(int contentId)
        {
            if (contentId == 0)
                return null;

            return _contentRepository.GetById(contentId);

        }

        /// <summary>
        /// Inserts a account content
        /// </summary>
        /// <param name="content">Account content</param>
        public virtual void InsertAccountContent(AccountContent content)
        {
            if (content == null)
                throw new ArgumentNullException("content");

            _contentRepository.Insert(content);

            //event notification
            //_eventPublisher.EntityInserted(content);
        }

        /// <summary>
        /// Updates a account content
        /// </summary>
        /// <param name="content">Account content</param>
        public virtual void UpdateAccountContent(AccountContent content)
        {
            if (content == null)
                throw new ArgumentNullException("content");

            _contentRepository.Update(content);

            //event notification
            //_eventPublisher.EntityUpdated(content);
        }

        #endregion
    }
}
