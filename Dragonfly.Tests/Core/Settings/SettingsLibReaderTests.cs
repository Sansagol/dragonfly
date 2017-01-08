using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dragonfly.Core.Settings;
using Dragonfly.SettingsLib;
using System.IO;

namespace Dragonfly.Tests.Core.Settings
{
    [TestClass]
    public class SettingsLibReaderTests
    {
        DatabaseConfig _DefaultConfiguration = new DatabaseConfig()
        {
            DbName = "Dragonfly.Test",
            DbAddress = "10.10.0.117",
            DefaultUserName = "Unit_Tester",
            DefaultUserPassword = "SelectAllPasswords"
        };

        SettingsLibReader _Raader = null;
        SettingsManager _LibManager = null;

        [TestInitialize]
        public void Initialize()
        {
            _Raader = new SettingsLibReader();
            _LibManager = new SettingsManager();
        }

        [TestMethod]
        public void ReadDbConfigurationTest()
        {
            Assert.IsNotNull(_LibManager, "Library settings manager can't be null, but it is.");

            DragonflyConfig currentConfig = null;

            currentConfig = ReadCurrentConfiguration();
            Assert.IsNotNull(
                currentConfig,
                "Return empty configuration after reading.");
            Assert.IsNotNull(
                currentConfig.DbConfiguration,
                "Return empty DB configuration after reading.");

            try
            {
                DatabaseAccessConfiguration accessConfig = _Raader.GetDbAccessSettings();
                Assert.IsNotNull(accessConfig, "Reader was returned an empty db config.");
                Assert.AreEqual(currentConfig.DbConfiguration.DbAddress, accessConfig.ServerName);
                Assert.AreEqual(currentConfig.DbConfiguration.DbName, accessConfig.DbName);
                Assert.AreEqual(currentConfig.DbConfiguration.DefaultUserName, accessConfig.UserName);
                Assert.AreEqual(currentConfig.DbConfiguration.DefaultUserPassword, accessConfig.Password);
            }
            finally
            {
            }
        }

        private DragonflyConfig ReadCurrentConfiguration()
        {
            DragonflyConfig currentConfig;
            try
            {
                currentConfig = _LibManager.LoadConfiguration();
            }
            catch (FileNotFoundException)
            {//Need create config in system.
                _LibManager.SaveConfiguration(new DragonflyConfig
                {
                    DbConfiguration = _DefaultConfiguration
                });
                currentConfig = _LibManager.LoadConfiguration();
            }

            return currentConfig;
        }
    }
}
