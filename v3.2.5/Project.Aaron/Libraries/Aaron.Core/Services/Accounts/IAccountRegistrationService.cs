using Aaron.Core.Domain.Accounts;

namespace Aaron.Core.Services.Accounts
{
    /// <summary>
    /// Account registration interface
    /// </summary>
    public partial interface IAccountRegistrationService
    {
        /// <summary>
        /// Validate account
        /// </summary>
        /// <param name="usernameOrEmail">Username or email</param>
        /// <param name="password">Password</param>
        /// <returns>Result</returns>
        bool ValidateAccount(string usernameOrEmail, string password);

        /// <summary>
        /// Register account
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Result</returns>
        AccountRegistrationResult RegisterAccount(AccountRegistrationRequest request);

        /// <summary>
        /// Change password
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Result</returns>
        PasswordChangeResult ChangePassword(ChangePasswordRequest request);

        /// <summary>
        /// Sets a user email
        /// </summary>
        /// <param name="account">Account</param>
        /// <param name="newEmail">New email</param>
        void SetEmail(Account account, string newEmail);

        /// <summary>
        /// Sets a account username
        /// </summary>
        /// <param name="account">Account</param>
        /// <param name="newUsername">New Username</param>
        void SetUsername(Account account, string newUsername);
    }
}