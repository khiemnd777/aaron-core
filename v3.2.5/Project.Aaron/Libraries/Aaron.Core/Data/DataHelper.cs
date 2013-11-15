using System;
using System.Data.SqlClient;
using Aaron.Core.Infrastructure;
using System.Threading;

namespace Aaron.Core.Data
{
    public partial class DataHelper
    {
        private static bool? _hasSettingsFileOrNotNull;
        private static bool? _databaseIsExisted;
        private static bool? _tableIsExisted;
        private static string connectionString;

        public static string ConnectionString { get{return connectionString;} }

        public static bool HasSettingsFileOrNotNull()
        {
            if (!_hasSettingsFileOrNotNull.HasValue)
            {
                var settings = IoC.Resolve<DataSettings>();
                _hasSettingsFileOrNotNull = settings != null && !String.IsNullOrEmpty(settings.ConnectionString);
            }
            return _hasSettingsFileOrNotNull.Value;
        }

        private static string _createConnectionString(bool trustedConnection, string serverName, string databaseName, string userName, string password, int timeOut = 0)
        {
            var builder = new SqlConnectionStringBuilder();
            builder.IntegratedSecurity = trustedConnection;
            builder.DataSource = serverName;
            builder.InitialCatalog = databaseName;
            if (!trustedConnection)
            {
                builder.UserID = userName;
                builder.Password = password;
            }
            builder.PersistSecurityInfo = false;
            builder.MultipleActiveResultSets = true;
            if (timeOut > 0)
            {
                builder.ConnectTimeout = timeOut;
            }

            connectionString = builder.ConnectionString;

            return connectionString;
        }

        public static void SaveSettings(string provider, bool trustedConnection = true, string serverName = ".\\SQLExpress", string databaseName = "", string userName = "", string password = "", int timeOut = 0)
        {
            var _connectionString = connectionString ?? _createConnectionString(trustedConnection, serverName, databaseName, userName, password, timeOut);

            var settings = new DataSettings() 
            { 
                ConnectionString = _connectionString,
                Provider = provider
            };

            var settingsManager = new DataSettingsManager();
            settingsManager.SaveSettings(settings);

            _hasSettingsFileOrNotNull = true;
        }

        public static void RemoveSettingsFile()
        {
            IoC.Resolve<DataSettingsManager>().RemoveSettingsFile();
        }

        public static bool CreateDatabase()
        {
            return CreateDatabase("SQL_Latin1_General_CP1_CI_AS");
        }

        public static bool CreateDatabase(string collation)
        {
            if (DatabaseIsExisted()) return false;

            var settings = IoC.Resolve<DataSettings>();

            var builder = new SqlConnectionStringBuilder(connectionString ?? settings.ConnectionString);
            var dbName = builder.InitialCatalog;
            builder.InitialCatalog = "master";
            var masterCatalogConnectionString = builder.ToString();
            string query = string.Format("CREATE DATABASE [{0}]", dbName);

            if (!String.IsNullOrWhiteSpace(collation))
                query = string.Format("{0} COLLATE {1}", query, collation);

            using (var conn = new SqlConnection(masterCatalogConnectionString))
            {
                conn.Open();
                using (var sqlCmd = new SqlCommand(query, conn))
                {
                    sqlCmd.ExecuteNonQuery();
                }
            }
            _databaseIsExisted = true;

            Thread.Sleep(3000);

            return true;
        }

        public static bool DatabaseIsExisted()
        {
            if (!HasSettingsFileOrNotNull()) return false;
            if (!_databaseIsExisted.HasValue)
            {
                var settings = IoC.Resolve<DataSettings>();
                using (var conn = new SqlConnection(settings.ConnectionString))
                {
                    try
                    {
                        conn.Open();
                        _databaseIsExisted = true;
                    }
                    catch
                    {
                        _databaseIsExisted = false;
                    }
                }
            }
            return _databaseIsExisted.Value;
        }

        public static bool TableIsExisted()
        {
            if (!DatabaseIsExisted()) return false;
            if (!_tableIsExisted.HasValue)
            {
                var settings = IoC.Resolve<DataSettings>();
                using (var conn = new SqlConnection(settings.ConnectionString))
                {
                    try
                    {
                        conn.Open();
                        var sqlCmd = new SqlCommand("SELECT COUNT(*) AS IsExists FROM dbo.sysobjects WHERE id=OBJECT_ID('[dbo].[Account]')", conn);
                        _tableIsExisted = (Int32)sqlCmd.ExecuteScalar() > 0;
                    }
                    catch
                    {
                        _tableIsExisted = false;
                    }
                }
            }
            return _tableIsExisted.Value;
        }

        public static void ResetCache()
        {
            _hasSettingsFileOrNotNull = null;
            _databaseIsExisted = null;
            _tableIsExisted = null;
            connectionString = null;
        }
    }
}
