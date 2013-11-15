using System;
using System.Collections.Generic;

namespace Aaron.Core.Data
{
    public partial class DataSettings
    {
        public DataSettings()
        {
            RawDataSettings = new Dictionary<string, string>();
        }
        public string Provider { get; set; }
        public string ConnectionString { get; set; }
        public IDictionary<string, string> RawDataSettings { get; private set; }

        public bool IsValid()
        {
            return !String.IsNullOrEmpty(this.Provider) && !String.IsNullOrEmpty(this.ConnectionString);
        }
    }
}
