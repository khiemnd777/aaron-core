using System.Net;
using Aaron.Core.Threading;

namespace Aaron.Core.Web
{
    public class KeepAliveTask : ITask
    {
        private readonly IWebHelper _webHelper;

        public KeepAliveTask(IWebHelper webHelper)
        {
            _webHelper = webHelper;
        }

        public void Execute()
        {
            var url = string.Format("{0}keepalive", _webHelper.GetWebLocation());
            using (var webClient = new WebClient())
            {
                webClient.DownloadString(url);
            }
        }
    }
}
