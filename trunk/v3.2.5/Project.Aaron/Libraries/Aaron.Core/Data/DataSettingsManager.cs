﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Hosting;

namespace Aaron.Core.Data
{
    public partial class DataSettingsManager
    {
        protected const string separator = "->";
        protected const string filename = "Connection.cfg";
        protected const string provider = "Provider";
        protected const string connectionString = "ConnectionString";
        private int separatorLength = separator.Length;
        /// <summary>
        /// Maps a virtual path to a physical disk path.
        /// </summary>
        /// <param name="path">The path to map. E.g. "~/bin"</param>
        /// <returns>The physical path. E.g. "c:\inetpub\wwwroot\bin"</returns>
        protected virtual string MapPath(string path)
        {
            if (HostingEnvironment.IsHosted)
            {
                //hosted
                return HostingEnvironment.MapPath(path);
            }
            else
            {
                //not hosted. For example, run in unit tests
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                path = path.Replace("~/", "").TrimStart('/').Replace('/', '\\');
                return Path.Combine(baseDirectory, path);
            }
        }

        protected virtual DataSettings ParseSettings(string text)
        {
            var shellSettings = new DataSettings();
            if (String.IsNullOrEmpty(text))
                return shellSettings;

            //Old way of file reading. This leads to unexpected behavior when a user's FTP program transfers these files as ASCII (\r\n becomes \n).
            //var settings = text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            var settings = new List<string>();
            using (var reader = new StringReader(text))
            {
                string str;
                while ((str = reader.ReadLine()) != null)
                    settings.Add(str);
            }

            foreach (var setting in settings)
            {
                var separatorIndex = setting.IndexOf(separator);
                if (separatorIndex == -1)
                {
                    continue;
                }
                string key = setting.Substring(0, separatorIndex).Trim();
                string value = setting.Substring(separatorIndex + separatorLength).Trim();

                switch (key)
                {
                    case provider:
                        shellSettings.Provider = value;
                        break;
                    case connectionString:
                        shellSettings.ConnectionString = value;
                        break;
                    default:
                        shellSettings.RawDataSettings.Add(key, value);
                        break;
                }
            }

            return shellSettings;
        }

        protected virtual string ComposeSettings(DataSettings settings)
        {
            if (settings == null)
                return "";

            return string.Format("{2}{4}{0}{5}{3}{4}{1}{5}", new object[]{
                                 settings.Provider,
                                 settings.ConnectionString,
                                 provider,
                                 connectionString,
                                 separator,
                                 Environment.NewLine
                                 }
                );
        }

        public virtual void RemoveSettingsFile()
        {
            string filePath = Path.Combine(MapPath("~/App_Data/"), filename);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        public virtual DataSettings LoadSettings()
        {
            //use webHelper.MapPath instead of HostingEnvironment.MapPath which is not available in unit tests
            string filePath = Path.Combine(MapPath("~/App_Data/"), filename);
            if (File.Exists(filePath))
            {
                string text = File.ReadAllText(filePath);
                return ParseSettings(text);
            }
            else
                return new DataSettings();
        }

        public virtual void SaveSettings(DataSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");

            //use webHelper.MapPath instead of HostingEnvironment.MapPath which is not available in unit tests
            string filePath = Path.Combine(MapPath("~/App_Data/"), filename);
            if (!File.Exists(filePath))
            {
                using (File.Create(filePath))
                {
                    //we use 'using' to close the file after it's created
                }
            }

            var text = ComposeSettings(settings);
            File.WriteAllText(filePath, text);
        }
    }
}
