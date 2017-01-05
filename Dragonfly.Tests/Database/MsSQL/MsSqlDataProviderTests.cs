using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dragonfly.Core;
using Dragonfly.Database.MsSQL;
using System.Data.Entity;
using Dragonfly.Core.Settings;
using Dragonfly.Database;
using Dragonfly.Models;

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
            LogUpModel model = new LogUpModel()
            {
                Login = "TestDbUser",
                Password = "testDbPassword",
                EMail = "some@mail.rrr"
            };
            try
            {
                Assert.IsTrue(provider.AddUser(model), "User not saved without error.");

                Assert.IsTrue(provider.CheckUserCredentials(model.Login, model.Password), "Wrong user saved");

                DragonflyEntities ents = context as DragonflyEntities;
                if (ents != null)
                {
                    User foundUser = (from u in ents.User
                                      where u.Name.Equals(model.Login)
                                      select u).FirstOrDefault();
                    if (foundUser != null)
                    {
                        Assert.AreEqual(model.EMail, foundUser.E_mail, "Wrong e-mail saved.");
                        //TODO check stored password
                    }
                }
            }
            finally
            {
                DeleteUserFromDB(context, model.Login);
                //TODO delete test user
            }
        }

        private void DeleteUserFromDB(DbContext context, string login)
        {
            DragonflyEntities ents = context as DragonflyEntities;
            if (ents != null)
            {
                var foundUsers = (from u in ents.User
                                  where u.Name.Equals(login)
                                  select u);
                foreach (var foundUser in foundUsers)
                {
                    ents.User.Remove(foundUser);
                }
                ents.SaveChanges();
            }
        }

        [TestMethod]
        public void AdduserWithoutEmailTest()
        {
            MsSqlDataProvider provider = new MsSqlDataProvider();
            DbContext context = provider.Initizlize(_Connectionconfig);

            LogUpModel model = new LogUpModel()
            {
                Login = "TestDbUser",
                Password = "testDbPassword"
            };
            try
            {
                Assert.IsFalse(provider.AddUser(model), "Inserted without e-mail");
            }
            finally
            {
                DeleteUserFromDB(context, model.Login);
                //TODO delete test user
            }
        }

        [TestMethod]
        public void AdduserWithDoubleEmailTest()
        {
            MsSqlDataProvider provider = new MsSqlDataProvider();
            DbContext context = provider.Initizlize(_Connectionconfig);

            LogUpModel model = new LogUpModel()
            {
                Login = "TestDbUser",
                Password = "testDbPassword",
                EMail = "some@mail.rrr"
            };
            try
            {
                Assert.IsTrue(provider.AddUser(model), "Insert failed");
                Assert.IsFalse(provider.AddUser(model), "Double insert equals user");
            }
            catch (InsertDbDataException ex)
            {
                Assert.IsTrue(ex.FieldNames.Count() > 0,
                    $"Exception thrown, but without error fields: {ex.Message}");
            }
            finally
            {
                DeleteUserFromDB(context, model.Login);
                //TODO delete test user
            }
        }
    }
}
