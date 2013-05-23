using System.Collections.Generic;
using Aaron.Core.SysConfiguration;

namespace Aaron.Core.Domain.Accounts
{
    public class ExternalAuthenticationSettings : ISettings
    {
        public ExternalAuthenticationSettings()
        {
            ActiveAuthenticationMethodSystemNames = new List<string>();
        }
        
        public bool AutoRegisterEnabled { get; set; }
        /// <summary>
        /// Gets or sets an system names of active payment methods
        /// </summary>
        public List<string> ActiveAuthenticationMethodSystemNames { get; set; }
    }
}