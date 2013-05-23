using System;
using System.Collections.Generic;
using System.Linq;
using Aaron.Core.Domain.Accounts;
using Aaron.Core.Domain.Messages;
using Aaron.Core.Services.Accounts;
using Aaron.Core.Services.Localization;
using Aaron.Core.Services.Messages;

namespace Aaron.Core.Services.Messages
{
    public partial class WorkflowMessageService : IWorkflowMessageService
    {
        #region Fields

        private readonly IMessageTemplateService _messageTemplateService;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly ITokenizer _tokenizer;
        private readonly ILanguageService _languageService;
        private readonly IEmailAccountService _emailAccountService;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly IMessageTokenProvider _messageTokenProvider;

        #endregion

        #region Ctor

        public WorkflowMessageService(IMessageTemplateService messageTemplateService,
            IQueuedEmailService queuedEmailService, 
            ILanguageService languageService,
            IEmailAccountService emailAccountService,
            EmailAccountSettings emailAccountSettings,
            ITokenizer tokenizer,
            IMessageTokenProvider messageTokenProvider)
        {
            _messageTemplateService = messageTemplateService;
            _queuedEmailService = queuedEmailService;
            _languageService = languageService;
            _emailAccountService = emailAccountService;
            _emailAccountSettings = emailAccountSettings;
            _tokenizer = tokenizer;
            _messageTokenProvider = messageTokenProvider;
        }

        #endregion

        #region Utilities
        
        private int SendNotification(MessageTemplate messageTemplate, 
            EmailAccount emailAccount, int languageId, IEnumerable<Token> tokens,
            string toEmailAddress, string toName)
        {
            //retrieve localized message template data
            var bcc = messageTemplate.GetLocalized((mt) => mt.BccEmailAddresses, languageId);
            var subject = messageTemplate.GetLocalized((mt) => mt.Subject, languageId);
            var body = messageTemplate.GetLocalized((mt) => mt.Body, languageId);

            //Replace subject and body tokens 
            var subjectReplaced = _tokenizer.Replace(subject, tokens, false);
            var bodyReplaced = _tokenizer.Replace(body, tokens, true);
            
            var email = new QueuedEmail()
            {
                Priority = 5,
                From = emailAccount.Email,
                FromName = emailAccount.DisplayName,
                To = toEmailAddress,
                ToName = toName,
                CC = string.Empty,
                Bcc = bcc,
                Subject = subjectReplaced,
                Body = bodyReplaced,
                CreatedOnUtc = DateTime.UtcNow,
                EmailAccountId = emailAccount.Id
            };

            _queuedEmailService.InsertQueuedEmail(email);
            return email.Id;
        }

        private IList<Token> GenerateTokens(Account account)
        {
            var tokens = new List<Token>();
            _messageTokenProvider.AddWebTokens(tokens);
            _messageTokenProvider.AddAccountTokens(tokens, account);
            return tokens;
        }

        private IList<Token> GenerateTokens(NewsLetterSubscription subscription)
        {
            var tokens = new List<Token>();

            _messageTokenProvider.AddWebTokens(tokens);
            _messageTokenProvider.AddNewsLetterSubscriptionTokens(tokens, subscription);

            return tokens;
        }

        private MessageTemplate GetLocalizedActiveMessageTemplate(string messageTemplateName, int languageId)
        {
            var messageTemplate = _messageTemplateService.GetMessageTemplateByName(messageTemplateName);
            if (messageTemplate == null)
                return null;

            //var isActive = messageTemplate.GetLocalized((mt) => mt.IsActive, languageId);
            //use
            var isActive = messageTemplate.IsActive;
            if (!isActive)
                return null;

            return messageTemplate;
        }

        private EmailAccount GetEmailAccountOfMessageTemplate(MessageTemplate messageTemplate, int languageId)
        {
            var emailAccounId = messageTemplate.GetLocalized(mt => mt.EmailAccountId, languageId);
            var emailAccount = _emailAccountService.GetEmailAccountById(emailAccounId);
            if (emailAccount == null)
                emailAccount = _emailAccountService.GetEmailAccountById(_emailAccountSettings.DefaultEmailAccountId);
            if (emailAccount == null)
                emailAccount = _emailAccountService.GetAllEmailAccounts().FirstOrDefault();
            return emailAccount;

        }

        private int EnsureLanguageIsActive(int languageId)
        {
            var language = _languageService.GetLanguageById(languageId);
            if (language == null || !language.Published)
                language = _languageService.GetAllLanguages().FirstOrDefault();
            return language.Id;
        }

        #endregion

        #region Methods

        #region Account workflow

        /// <summary>
        /// Sends 'New account' notification message to a store owner
        /// </summary>
        /// <param name="account">Account instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendAccountRegisteredNotificationMessage(Account account, int languageId)
        {
            if (account == null)
                throw new ArgumentNullException("account");

            languageId = EnsureLanguageIsActive(languageId);

            var messageTemplate = GetLocalizedActiveMessageTemplate("NewAccount.Notification", languageId);
            if (messageTemplate == null)
                return 0;

            var accountTokens = GenerateTokens(account);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = emailAccount.Email;
            var toName = emailAccount.DisplayName;
            return SendNotification(messageTemplate, emailAccount,
                languageId, accountTokens,
                toEmail, toName);
        }

        /// <summary>
        /// Sends a welcome message to a account
        /// </summary>
        /// <param name="account">Account instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendAccountWelcomeMessage(Account account, int languageId)
        {
            if (account == null)
                throw new ArgumentNullException("account");

            languageId = EnsureLanguageIsActive(languageId);

            var messageTemplate = GetLocalizedActiveMessageTemplate("Account.WelcomeMessage", languageId);
            if (messageTemplate == null)
                return 0;

            var accountTokens = GenerateTokens(account);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = account.Email;
            var toName = account.GetFullName();
            return SendNotification(messageTemplate, emailAccount,
                languageId, accountTokens, 
                toEmail, toName);
        }

        /// <summary>
        /// Sends an email validation message to a account
        /// </summary>
        /// <param name="account">Account instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendAccountEmailValidationMessage(Account account, int languageId)
        {
            if (account == null)
                throw new ArgumentNullException("account");

            languageId = EnsureLanguageIsActive(languageId);

            var messageTemplate = GetLocalizedActiveMessageTemplate("Account.EmailValidationMessage", languageId);
            if (messageTemplate == null)
                return 0;

            var accountTokens = GenerateTokens(account);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = account.Email;
            var toName = account.GetFullName();
            return SendNotification(messageTemplate, emailAccount,
                languageId, accountTokens,
                toEmail, toName);
        }

        /// <summary>
        /// Sends password recovery message to a account
        /// </summary>
        /// <param name="account">Account instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendAccountPasswordRecoveryMessage(Account account, int languageId)
        {
            if (account == null)
                throw new ArgumentNullException("account");
            
            languageId = EnsureLanguageIsActive(languageId);

            var messageTemplate = GetLocalizedActiveMessageTemplate("Account.PasswordRecovery", languageId);
            if (messageTemplate == null)
                return 0;

            var accountTokens = GenerateTokens(account);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = account.Email;
            var toName = account.GetFullName();
            return SendNotification(messageTemplate, emailAccount,
                languageId, accountTokens,
                toEmail, toName);
        }

        #endregion

        #region Newsletter workflow

        /// <summary>
        /// Sends a newsletter subscription activation message
        /// </summary>
        /// <param name="subscription">Newsletter subscription</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendNewsLetterSubscriptionActivationMessage(NewsLetterSubscription subscription,
            int languageId)
        {
            if (subscription == null)
                throw new ArgumentNullException("subscription");

            languageId = EnsureLanguageIsActive(languageId);

            var messageTemplate = GetLocalizedActiveMessageTemplate("NewsLetterSubscription.ActivationMessage", languageId);
            if (messageTemplate == null)
                return 0;

            var orderTokens = GenerateTokens(subscription);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = subscription.Email;
            var toName = "";
            return SendNotification(messageTemplate, emailAccount,
                languageId, orderTokens,
                toEmail, toName);
        }

        /// <summary>
        /// Sends a newsletter subscription deactivation message
        /// </summary>
        /// <param name="subscription">Newsletter subscription</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendNewsLetterSubscriptionDeactivationMessage(NewsLetterSubscription subscription,
            int languageId)
        {
            if (subscription == null)
                throw new ArgumentNullException("subscription");

            languageId = EnsureLanguageIsActive(languageId);

            var messageTemplate = GetLocalizedActiveMessageTemplate("NewsLetterSubscription.DeactivationMessage", languageId);
            if (messageTemplate == null)
                return 0;

            var orderTokens = GenerateTokens(subscription);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = subscription.Email;
            var toName = "";
            return SendNotification(messageTemplate, emailAccount,
                languageId, orderTokens,
                toEmail, toName);
        }

        #endregion

        #endregion
    }
}