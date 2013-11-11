using System.Net;
using Aaron.Core.Domain;
using Aaron.Core.Threading;
using Aaron.Core.Web;

namespace Aaron.Core.Services.Common
{
    /// <summary>
    /// Represents a task for keeping the site alive
    /// </summary>
    public partial class KeepAliveTask : ITask
    {
        private readonly WebInformationSettings _webInformationSettings;
        public KeepAliveTask(WebInformationSettings webInformationSettings)
        {
            this._webInformationSettings = webInformationSettings;
        }
        /// <summary>
        /// Executes a task
        /// </summary>
        public void Execute()
        {
            string url = _webInformationSettings.WebUrl + "keepalive";
            using (var wc = new WebClient())
            {
                wc.DownloadString(url);
            }
        }
    }
}
