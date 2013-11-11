
namespace Aaron.Core.Services.Common
{
    /// <summary>
    /// Full-Text service interface
    /// </summary>
    public partial interface IFulltextService : IServices
    {
        /// <summary>
        /// Gets value indicating whether Full-Text is supported
        /// </summary>
        /// <returns>Result</returns>
        bool IsFullTextSupported();

        /// <summary>
        /// Enable Full-Text support
        /// </summary>
        void EnableFullText();

        /// <summary>
        /// Disable Full-Text support
        /// </summary>
        void DisableFullText();
    }
}
