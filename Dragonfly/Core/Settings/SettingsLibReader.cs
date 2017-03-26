using Dragonfly.SettingsLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dragonfly.Core.Settings
{
    public class SettingsLibReader : ISettingsReader
    {
        SettingsManager _Manager = null;
        public SettingsLibReader()
        {
            _Manager = new SettingsManager();
        }

        /// <summary>Method create a settings to database access.</summary>
        /// <returns>Settings to access to database.</returns>
        /// <exception cref="InvalidOperationException">
        /// Unable to generate settings. Something wrong.
        /// </exception>
        public DatabaseAccessConfiguration GetDbAccessSettings()
        {
            return GetDbAccessSettings(null);
        }

        /// <summary>Method create a settings to database access.</summary>
        /// <returns>Settings to access to database.</returns>
        /// <exception cref="InvalidOperationException">
        /// Unable to generate settings. Something wrong.
        /// </exception>
        public DatabaseAccessConfiguration GetDbAccessSettings(string configPath)
        {
            DragonflyConfig fullConfig = LoadConfigurationFile(configPath);

            DatabaseAccessConfiguration dbConfig = new DatabaseAccessConfiguration()
            {
                ServerName = fullConfig.DbConfiguration.DbAddress,
                UserName = fullConfig.DbConfiguration.DefaultUserName,
                Password = fullConfig.DbConfiguration.DefaultUserPassword,
                DbName = fullConfig.DbConfiguration.DbName
            };
            return dbConfig;
        }

        private DragonflyConfig LoadConfigurationFile(string configPath)
        {
            if (_Manager == null)
                throw new InvalidOperationException("Settings manager not loaded.");

            DragonflyConfig fullConfig = _Manager.LoadConfiguration(configPath);
            if (fullConfig.DbConfiguration == null)
            {
                throw new InvalidOperationException(
                    "database configuraion not presented in config file.");
            }

            return fullConfig;
        }

        public string GetLogDirectory()
        {
            DragonflyConfig fullConfig = LoadConfigurationFile(string.Empty);
            return fullConfig.LogDirectory;
        }
    }
}