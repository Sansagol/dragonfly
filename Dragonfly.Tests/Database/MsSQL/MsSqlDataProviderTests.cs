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
using Dragonfly.Database.Providers;
using Dragonfly.Models.UserRoleSystem;
using Dragonfly.Database.Entities;

namespace Dragonfly.Tests.Database.MsSQL
{
    [TestClass]
    public class MsSqlDataProviderTests
    {
        static SignUpModel _UserSignUpData = null;

        [ClassInitialize]
        public static void InitTests(TestContext context)
        {
            _UserSignUpData = new SignUpModel()
            {
                Login = "Test_user",
                EMail = "test@mail.mail",
                Password = "Test user password"
            };

        }

        [TestMethod]
        public void InitializeConnection()
        {
            MsSqlFactory factory = new MsSqlFactory(Common.Connectionconfig);
            MsSqlDataProvider provider = factory.CreateDBProvider() as MsSqlDataProvider;
            using (var context = provider.GenerateContext())
            {
                Assert.IsNotNull(context, "Context not created.");
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
            MsSqlFactory factory = new MsSqlFactory(Common.Connectionconfig);
            DbContext context = null;
            try
            {
                MsSqlDataProvider provider =
                    factory.CreateDBProvider() as MsSqlDataProvider;
                context = provider.GenerateContext();
                Assert.IsNull(context, "Context created - is wrong.");
            }
            finally
            {
                Common.Connectionconfig.DbName = "Dragonfly.Test";
                context?.Dispose();
            }
        }

        [TestMethod]
        public void AddUserTest()
        {
            MsSqlFactory factory = new MsSqlFactory(Common.Connectionconfig);
            MsSqlDataProvider provider = factory.CreateDBProvider() as MsSqlDataProvider;
            using (DbContext context = provider.GenerateContext())
            {
                SignUpModel model = new SignUpModel()
                {
                    Login = "TestDbUser",
                    Password = "testDbPassword",
                    EMail = "some@mail.rrr"
                };
                try
                {
                    Assert.IsTrue(provider.AddUser(model) > 0, "User not saved without error.");

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
            MsSqlFactory factory = new MsSqlFactory(Common.Connectionconfig);
            MsSqlDataProvider provider = factory.CreateDBProvider() as MsSqlDataProvider;
            using (DbContext context = provider.GenerateContext())
            {
                SignUpModel model = new SignUpModel()
                {
                    Login = "TestDbUser",
                    Password = "testDbPassword"
                };
                try
                {
                    Assert.IsFalse(provider.AddUser(model) > 0, "Inserted without e-mail");
                }
                finally
                {
                    DeleteUserFromDB(context, model.Login, model.EMail);
                    //TODO delete test user
                }
            }
        }

        [TestMethod]
        public void AddUserWithDoubleEmailTest()
        {
            MsSqlFactory factory = new MsSqlFactory(Common.Connectionconfig);
            MsSqlDataProvider provider = factory.CreateDBProvider() as MsSqlDataProvider;
            using (DbContext context = provider.GenerateContext())
            {
                SignUpModel model = new SignUpModel()
                {
                    Login = "TestDbUser",
                    Password = "testDbPassword",
                    EMail = "some@mail.rrr"
                };
                try
                {
                    Assert.IsTrue(provider.AddUser(model) > 0, "Insert failed");
                    Assert.IsFalse(provider.AddUser(model) > 0, "Double insert equals user");
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
        }

        /// <summary>Checking simple project creating.</summary>
        [TestMethod]
        public void CreateProjectTest()
        {
            MsSqlFactory factory = new MsSqlFactory(Common.Connectionconfig);
            MsSqlDataProvider provider = factory.CreateDBProvider() as MsSqlDataProvider;
            IUserAccessProvider userProvider = factory.CreateUserAccessProvider();
            SignUpModel userData = new SignUpModel()
            {
                Login = "Test_user",
                EMail = "test@mail.mail",
                Password = "Test user password"
            };
            provider.AddUser(userData);
            EUser userModel = userProvider.GetUserByLoginMail("test@mail.mail");

            ProjectModel model = new ProjectModel(provider)
            {
                ProjectDetails = new EProject()
                {
                    Description = "Project description",
                    ProjectName = "Test project name"
                },
                //UserIds = new System.Collections.Generic.List<decimal>()
                //{
                //    userModel.Id
                //}
            };

            try
            {
                provider.CreateProject(model);
                Assert.IsTrue(model.ProjectDetails.Id > 0, "Project id less than 1.");

                ProjectModel selectedProjectModel = new ProjectModel()
                {
                    ProjectDetails = provider.GetProjectById(model.ProjectDetails.Id)
                };
                Assert.IsNotNull(
                    selectedProjectModel,
                    $"Unable to retrieve project with id \'{model.ProjectDetails.Id}\'");
                Assert.AreEqual(
                    model.ProjectDetails.ProjectName,
                    selectedProjectModel.ProjectDetails.ProjectName);
                Assert.AreEqual(
                    model.ProjectDetails.Description,
                    selectedProjectModel.ProjectDetails.Description);

                //Check users
                //using (DragonflyEntities ents = provider.GenerateContext())
                //{
                //    var projectUsers = (from usr in ents.User_Project
                //                        where usr.ID_Project == selectedProjectModel.ProjectId
                //                        select usr).ToList();
                //    Assert.IsTrue(projectUsers.All(pu => model.UserIds.Contains(pu.ID_User)),
                //        "Not all users added to project management.");
                //}
            }
            finally
            {
                if (model != null && model.ProjectDetails.Id > 0)
                    provider.DeleteProject(model.ProjectDetails.Id);
                using (DragonflyEntities ents = provider.GenerateContext())
                {
                    DeleteUserFromDB(ents, userData.Login, userData.EMail);
                }
            }
        }

        /// <summary>
        /// Methot check creation of the access token for user.
        /// </summary>
        [TestMethod]
        public void CreateAccessTokenTest()
        {
            MsSqlFactory factory = new MsSqlFactory(Common.Connectionconfig);
            MsSqlDataProvider provider = factory.CreateDBProvider() as MsSqlDataProvider;
            IUserAccessProvider accessProvider = factory.CreateUserAccessProvider();
            using (DbContext context = provider.GenerateContext())
            {
                DragonflyEntities ents = context as DragonflyEntities;
                decimal createdAceessToken = 0;

                try
                {
                    decimal userId = provider.AddUser(_UserSignUpData);
                    Assert.IsTrue(userId > 0, "Error occured on the user save.");
                    string token = accessProvider.CreateAccessToken(userId);
                    var accessTokens = (from at in ents.User_Access
                                        where at.ID_User == userId
                                        select at);
                    if (accessTokens.Count() > 1)
                    {
                        foreach (var foundToken in accessTokens)
                            DeleteAccessTokenFromDB(context, foundToken.ID_User_Access);
                        Assert.Fail("Too many access tokens");
                    }
                    else if (accessTokens.Count() == 1)
                    {
                        createdAceessToken = accessTokens.First().ID_User_Access;
                        Assert.AreEqual(token, accessTokens.First().Access_Token);
                    }
                    else
                        Assert.Fail("Access tokens not found in the DB.");
                }
                finally
                {
                    DeleteUserFromDB(context, _UserSignUpData.Login, _UserSignUpData.EMail);
                    DeleteAccessTokenFromDB(context, createdAceessToken);
                }
            }
        }

        private void DeleteAccessTokenFromDB(DbContext context, decimal id)
        {
            if (id > 0)
            {
                DragonflyEntities ents = context as DragonflyEntities;
                if (ents != null)
                {
                    var foundTokens = (from u in ents.User_Access
                                       where u.ID_User_Access == id
                                       select u);
                    foreach (var foundUser in foundTokens)
                    {
                        ents.User_Access.Remove(foundUser);
                    }
                    ents.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Method check whether all tokens delete from the DB when a userd delete.
        /// </summary>
        [TestMethod]
        public void AccessTokenCascadeDeletionTest()
        {
            MsSqlFactory factory = new MsSqlFactory(Common.Connectionconfig);
            MsSqlDataProvider provider = factory.CreateDBProvider() as MsSqlDataProvider;
            IUserAccessProvider accessProvider = factory.CreateUserAccessProvider();
            using (DbContext context = provider.GenerateContext())
            {
                DragonflyEntities ents = context as DragonflyEntities;
                decimal createdAceessToken = 0;

                try
                {
                    decimal userId = provider.AddUser(_UserSignUpData);
                    Assert.IsTrue(userId > 0, "Error occured on the user save.");
                    string token = accessProvider.CreateAccessToken(userId);
                    var accessTokens = (from at in ents.User_Access
                                        where at.ID_User == userId
                                        select at);
                    if (accessTokens.Count() == 1)
                    {
                        createdAceessToken = accessTokens.First().ID_User_Access;
                        DeleteUserFromDB(context, _UserSignUpData.Login, _UserSignUpData.EMail);
                        var tokensCount = (from at in ents.User_Access
                                           where at.ID_User == userId
                                           select at).Count();
                        Assert.AreEqual(0, tokensCount, "Access tokens for user stay in the DB");
                    }
                }
                finally
                {
                    DeleteUserFromDB(context, _UserSignUpData.Login, _UserSignUpData.EMail);
                    DeleteAccessTokenFromDB(context, createdAceessToken);
                }
            }
        }

        [TestMethod]
        public void CheckAccessTokenTest()
        {
            MsSqlFactory factory = new MsSqlFactory(Common.Connectionconfig);
            MsSqlDataProvider provider = factory.CreateDBProvider() as MsSqlDataProvider;
            IUserAccessProvider accessProvider = factory.CreateUserAccessProvider();
            using (DbContext context = provider.GenerateContext())
            {
                DragonflyEntities ents = context as DragonflyEntities;
                try
                {
                    decimal userId = provider.AddUser(_UserSignUpData);
                    Assert.IsTrue(userId > 0, "Error occured on the user save.");
                    string token = accessProvider.CreateAccessToken(userId);
                    Assert.IsTrue(accessProvider.CheckAccessToken(userId, token));
                }
                finally
                {
                    DeleteUserFromDB(context, _UserSignUpData.Login, _UserSignUpData.EMail);
                }
            }
        }
    }
}
