using Aaron.Core.Domain.Accounts;
using Aaron.Core.Domain.Localization;

namespace Aaron.Core
{
    public interface ICurrentActivity
    {
        /// <summary>
        /// Gets or sets the current customer
        /// </summary>
        Account CurrentAccount { get; set; }

        /// <summary>
        /// Gets or sets the original customer (in case the current one is impersonated)
        /// </summary>
        Account OriginalAccountIfImpersonated { get; }
        /// <summary>
        /// Get or set current user working language
        /// </summary>
        Language CurrentLanguage { get; set; }

        /// <summary>
        /// Get or set value indicating whether we're in admin area
        /// </summary>
        bool IsAdmin { get; set; }
    }
}
