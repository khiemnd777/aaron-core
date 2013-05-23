//Contributor:  Nicholas Mayne

using System;
using System.Collections.Generic;
using System.Linq;
using Aaron.Core.Data;
using Aaron.Core.Domain.Accounts;
using Aaron.Core.Plugins;
using Aaron.Core.Services.Accounts;

namespace Aaron.Core.Services.Authentication.External
{
    public partial class OpenAuthenticationService : IOpenAuthenticationService
    {
        private readonly IAccountService _accountService;
        private readonly IPluginFinder _pluginFinder;
        private readonly ExternalAuthenticationSettings _externalAuthenticationSettings;
        private readonly IRepository<ExternalAuthenticationRecord> _externalAuthenticationRecordRepository;

        public OpenAuthenticationService(IRepository<ExternalAuthenticationRecord> externalAuthenticationRecordRepository,
            IPluginFinder pluginFinder,
            ExternalAuthenticationSettings externalAuthenticationSettings,
            IAccountService accountService)
        {
            this._externalAuthenticationRecordRepository = externalAuthenticationRecordRepository;
            this._pluginFinder = pluginFinder;
            this._externalAuthenticationSettings = externalAuthenticationSettings;
            this._accountService = accountService;
        }

        /// <summary>
        /// Load active external authentication methods
        /// </summary>
        /// <returns>Payment methods</returns>
        public virtual IList<IExternalAuthenticationMethod> LoadActiveExternalAuthenticationMethods()
        {
            return LoadAllExternalAuthenticationMethods()
                   .Where(provider => _externalAuthenticationSettings.ActiveAuthenticationMethodSystemNames.Contains(provider.PluginDescriptor.SystemName, StringComparer.InvariantCultureIgnoreCase))
                   .ToList();
        }

        /// <summary>
        /// Load external authentication method by system name
        /// </summary>
        /// <param name="systemName">System name</param>
        /// <returns>Found external authentication method</returns>
        public virtual IExternalAuthenticationMethod LoadExternalAuthenticationMethodBySystemName(string systemName)
        {
            var descriptor = _pluginFinder.GetPluginDescriptorBySystemName<IExternalAuthenticationMethod>(systemName);
            if (descriptor != null)
                return descriptor.Instance<IExternalAuthenticationMethod>();

            return null;
        }

        /// <summary>
        /// Load all external authentication methods
        /// </summary>
        /// <returns>External authentication methods</returns>
        public virtual IList<IExternalAuthenticationMethod> LoadAllExternalAuthenticationMethods()
        {
            return _pluginFinder.GetPlugins<IExternalAuthenticationMethod>().ToList();
        }




        public virtual void AssociateExternalAccountWithUser(Account account, OpenAuthenticationParameters parameters)
        {
            if (account == null)
                throw new ArgumentNullException("account");

            //find email
            string email = null;
            if (parameters.UserClaims != null)
                foreach (var userClaim in parameters.UserClaims
                    .Where(x => x.Contact != null && !String.IsNullOrEmpty(x.Contact.Email)))
                    {
                        //found
                        email = userClaim.Contact.Email;
                        break;
                    }

            var externalAuthenticationRecord = new ExternalAuthenticationRecord()
            {
                AccountId = account.Id,
                Email = email,
                ExternalIdentifier = parameters.ExternalIdentifier,
                ExternalDisplayIdentifier = parameters.ExternalDisplayIdentifier,
                OAuthToken = parameters.OAuthToken,
                OAuthAccessToken = parameters.OAuthAccessToken,
                ProviderSystemName = parameters.ProviderSystemName,
            };

            _externalAuthenticationRecordRepository.Insert(externalAuthenticationRecord);
        }

        public virtual bool AccountExists(OpenAuthenticationParameters parameters)
        {
            return GetUser(parameters) != null;
        }

        public virtual Account GetUser(OpenAuthenticationParameters parameters)
        {
            var record = _externalAuthenticationRecordRepository.Table
                .Where(o => o.ExternalIdentifier == parameters.ExternalIdentifier && o.ProviderSystemName == parameters.ProviderSystemName)
                .FirstOrDefault();

            if (record != null)
                return _accountService.GetAccountById(record.AccountId);

            return null;
        }

        public virtual IList<ExternalAuthenticationRecord> GetExternalIdentifiersFor(Account account)
        {
            if (account == null)
                throw new ArgumentNullException("account");

            return account.ExternalAuthenticationRecords.ToList();
        }

        public virtual void RemoveAssociation(OpenAuthenticationParameters parameters)
        {
            var record = _externalAuthenticationRecordRepository.Table
                .Where(o => o.ExternalIdentifier == parameters.ExternalIdentifier && o.ProviderSystemName == parameters.ProviderSystemName)
                .FirstOrDefault();

            if (record != null)
                _externalAuthenticationRecordRepository.Delete(record);
        }
    }
}