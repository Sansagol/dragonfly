using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dragonfly.Core;
using Dragonfly.Database.MsSQL;
using System.Data.Entity;

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
            DbContext context = provider.GetNewContext(_Connectionconfig);
            try
            {
                Assert.IsNotNull(context, "Context not created.");
            }
            finally
            {
                if (context != null)
                    context.Dispose();
            }
        }

        [TestMethod]
        public void WrongDbNameConnectionTest()
        {
            _Connectionconfig.DbName = "Dragonfly";
            MsSqlDataProvider provider = new MsSqlDataProvider();
            DbContext context= provider.GetNewContext(_Connectionconfig);
            try
            {
                Assert.IsNull(context, "Context created - is wrong.");
            }
            finally
            {
                if (context != null)
                    context.Dispose();
            }
        }
    }
}
