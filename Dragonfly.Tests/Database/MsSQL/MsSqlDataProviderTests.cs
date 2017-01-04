using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dragonfly.Core;
using Dragonfly.Database.MsSQL;
using System.Data.Entity;
using Dragonfly.Core.Settings;
using Dragonfly.Database;

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
            DbContext context = provider.Initizlize(_Connectionconfig);
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
            DbContext context = provider.Initizlize(_Connectionconfig);
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

        [TestMethod]
        public void AdduserTest()
        {
            MsSqlDataProvider provider = new MsSqlDataProvider();
            DbContext context = provider.Initizlize(_Connectionconfig);

            string login = "TestDbUser";
            string password = "testDbPassword";
            try
            {
                Assert.IsTrue(provider.AddUser(login, password), "User not saved without error.");

                Assert.IsTrue(provider.CheckUserCredentials(login, password), "Wrong user saved");
            }
            finally
            {
                //TODO delete test user
            }
        }

        [TestMethod]
        public void AdduserWithExceptionTest()
        {
            MsSqlDataProvider provider = new MsSqlDataProvider();
            DbContext context = provider.Initizlize(_Connectionconfig);

            string login = "TestDbUser";
            string password = "testDbPassword";
            try
            {
                Assert.IsTrue(provider.AddUser(login, password), "User not saved without error.");
            }
            catch (InsertDbDataException ex)
            {
                Assert.IsTrue(ex.FieldNames.Count() > 0,
                    $"Exception thrown, but without error fields: {ex.Message}");

            }
            finally
            {
                //TODO delete test user
            }
        }
    }
}
