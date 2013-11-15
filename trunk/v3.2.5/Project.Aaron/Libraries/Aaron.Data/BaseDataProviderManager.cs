﻿using System;
using Aaron.Core;
using Aaron.Core.Data;
using Aaron.Data.SqlServer;

namespace Aaron.Data
{
    public partial class BaseDataProviderManager : DataProviderManager
    {
        public BaseDataProviderManager(DataSettings settings)
            : base(settings)
        {
        }

        public override IDataProvider LoadDataProvider()
        {
            var providerName = Settings.Provider;
            if (String.IsNullOrWhiteSpace(providerName))
                throw new Exception("Data Settings doesn't contain a providerName");

            switch (providerName.ToLowerInvariant())
            {
                case "sqlserver":
                    return new SqlServerDataProvider();
                case "sqlce":
                    //return new SqlCeDataProvider();
                default:
                    throw new Exception(string.Format("Not supported dataprovider name: {0}", providerName));
            }
        }

    }
}
