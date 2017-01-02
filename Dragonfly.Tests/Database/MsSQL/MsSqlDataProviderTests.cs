using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dragonfly.Core;
using Dragonfly.Database.MsSQL;

namespace Dragonfly.Tests.Database.MsSQL
{
    [TestClass]
    public class MsSqlDataProviderTests
    {
        DatabaseAccessConfiguration _Connectionconfig = new DatabaseAccessConfiguration()
        {
            DbName = "Dragonfly.Test",
            ServerName = "10.10.0.117",
            UserName = "Unit_Tester",
            Password = "SelectAllPasswords"
        };

        [TestMethod]
        public void InitializeConnection()
        {
            MsSqlDataProvider provider = new MsSqlDataProvider();
            Assert.IsTrue(provider.InitializeConnection(_Connectionconfig));
        }

        [TestMethod]
        public void BadConnection()
        {
            _Connectionconfig.DbName = "Dragonfly";
            MsSqlDataProvider provider = new MsSqlDataProvider();
            Assert.IsFalse(provider.InitializeConnection(_Connectionconfig));
        }
    }
}
