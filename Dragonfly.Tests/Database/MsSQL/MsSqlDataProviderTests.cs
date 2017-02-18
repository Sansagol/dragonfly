using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dragonfly.Core;
using Dragonfly.Database.MsSQL;
using System.Data.Entity;
using Dragonfly.Core.Settings;
using Dragonfly.Database;
using Dragonfly.Models;
using Dragonfly.Models.Projects;

namespace Dragonfly.Tests.Database.MsSQL
{
    [TestClass]
    public class MsSqlDataProviderTests
    {
        [TestMethod]
        public void InitializeConnection()
        {
            MsSqlDataProvider provider = new MsSqlDataProvider();
            DbContext context = provider.Initizlize(Common.Connectionconfig);
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

        /// <summary>
        /// Check if context with wrong credentials will not create.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(DbInitializationException))]
        public void WrongDbNameConnectionTest()
        {
            Common.Connectionconfig.DbName = "Dragonfly";
            MsSqlDataProvider provider = new MsSqlDataProvider();
            DbContext context = null;
            try
            {
                context = provider.Initizlize(Common.Connectionconfig);
                Assert.IsNull(context, "Context created - is wrong.");
            }
            finally
            {
                Common.Connectionconfig.DbName = "Dragonfly.Test";
                if (context != null)
                    context.Dispose();
            }
        }

        [TestMethod]
        public void AddUserTest()
        {
            MsSqlDataProvider provider = new MsSqlDataProvider();
            DbContext context = provider.Initizlize(Common.Connectionconfig);
            LogUpModel model = new LogUpModel()
            {
                Login = "TestDbUser",
                Password = "testDbPassword",
                EMail = "some@mail.rrr"
            };
            try
            {
                Assert.IsTrue(provider.AddUser(model), "User not saved without error.");

                Assert.IsTrue(provider.CheckUserCredentials(model.Login, model.Password), "Wrong user saved (login)");
                Assert.IsTrue(provider.CheckUserCredentials(model.EMail, model.Password), "Wrong user saved (email)");


                DragonflyEntities ents = context as DragonflyEntities;
                if (ents != null)
                {
                    User foundUser = (from u in ents.User
                                      where u.Login.Equals(model.Login)
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
                DeleteUserFromDB(context, model.Login, model.EMail);
                //TODO delete test user
            }
        }

        private void DeleteUserFromDB(DbContext context, string login, string eMail)
        {
            DragonflyEntities ents = context as DragonflyEntities;
            if (ents != null)
            {
                var foundUsers = (from u in ents.User
                                  where u.Login.Equals(login) || u.E_mail.Equals(eMail)
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
            DbContext context = provider.Initizlize(Common.Connectionconfig);

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
                DeleteUserFromDB(context, model.Login, model.EMail);
                //TODO delete test user
            }
        }

        [TestMethod]
        public void AddUserWithDoubleEmailTest()
        {
            MsSqlDataProvider provider = new MsSqlDataProvider();
            DbContext context = provider.Initizlize(Common.Connectionconfig);

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
                DeleteUserFromDB(context, model.Login, model.EMail);
                //TODO delete test user
            }
        }

        /// <summary>Checking simple project creating.</summary>
        [TestMethod]
        public void CreateProjectTest()
        {
            MsSqlDataProvider provider = new MsSqlDataProvider();
            DbContext context = provider.Initizlize(Common.Connectionconfig);
            LogUpModel userData = new LogUpModel()
            {
                Login = "Test_user",
                EMail = "test@mail.mail",
                Password = "Test user password"
            };
            provider.AddUser(userData);
            UserModel userModel = provider.GetUserByLoginMail("test@mail.mail");

            ProjectModel model = new ProjectModel(provider)
            {
                Description = "Project description",
                ProjectName = "Test project name",
                UserIds = new System.Collections.Generic.List<decimal>()
                {
                    userModel.Id
                }
            };

            try
            {
                provider.CreateProject(model);
                Assert.IsTrue(model.ProjectId > 0, "Project id less than 1.");

                ProjectModel selectedProjectModel = provider.GetProjectById(model.ProjectId);
                Assert.IsNotNull(
                    selectedProjectModel,
                    $"Unable to retrieve project with id \'{model.ProjectId}\'");
                Assert.AreEqual(model.ProjectName, selectedProjectModel.ProjectName);
                Assert.AreEqual(model.Description, selectedProjectModel.Description);

                //Check users
                DragonflyEntities ents = context as DragonflyEntities;
                var projectUsers = (from usr in ents.User_Project
                                    where usr.ID_Project == selectedProjectModel.ProjectId
                                    select usr).ToList();
                Assert.IsTrue(projectUsers.All(pu => model.UserIds.Contains(pu.ID_User)),
                    "Not all users added to project management.");
            }
            finally
            {
                if (model != null && model.ProjectId > 0)
                    provider.DeleteProject(model.ProjectId);
                DeleteUserFromDB(context, userData.Login, userData.EMail);
            }
        }
    }
}
