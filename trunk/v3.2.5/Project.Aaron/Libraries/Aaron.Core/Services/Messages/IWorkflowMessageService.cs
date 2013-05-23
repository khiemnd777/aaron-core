using Aaron.Core.Domain.Accounts;
using Aaron.Core.Domain.Messages;
using Aaron.Core;

namespace Aaron.Core.Services.Messages
{
    public partial interface IWorkflowMessageService : IServices
    {
        #region Account workflow

        /// <summary>
        /// Sends 'New account' notification message to a store owner
        /// </summary>
        /// <param name="account">Account instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        int SendAccountRegisteredNotificationMessage(Account account, int languageId);

        /// <summary>
        /// Sends a welcome message to a account
        /// </summary>
        /// <param name="account">Account instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        int SendAccountWelcomeMessage(Account account, int languageId);

        /// <summary>
        /// Sends an email validation message to a account
        /// </summary>
        /// <param name="account">Account instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        int SendAccountEmailValidationMessage(Account account, int languageId);

        /// <summary>
        /// Sends password recovery message to a account
        /// </summary>
        /// <param name="account">Account instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        int SendAccountPasswordRecoveryMessage(Account account, int languageId);
        
        #endregion

        #region Newsletter workflow

        /// <summary>
        /// Sends a newsletter subscription activation message
        /// </summary>
        /// <param name="subscription">Newsletter subscription</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Queued email identifier</returns>
        int SendNewsLetterSubscriptionActivationMessage(NewsLetterSubscription subscription,
            int languageId);

        /// <summary>
        /// Sends a newsletter subscription deactivation message
        /// </summary>
        /// <param name="subscription">Newsletter subscription</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Queued email identifier</returns>
        int SendNewsLetterSubscriptionDeactivationMessage(NewsLetterSubscription subscription,
            int languageId);

        #endregion
    }
}
