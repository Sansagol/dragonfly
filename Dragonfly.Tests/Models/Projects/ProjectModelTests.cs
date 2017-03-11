using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dragonfly.Models.Projects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dragonfly.Database.MsSQL;
using System.Data.Entity;
using Dragonfly.Tests;
using Dragonfly.Database;
using Dragonfly.Database.Providers;

namespace Dragonfly.Models.Projects.Tests
{
    [TestClass()]
    public class ProjectModelTests
    {
        /// <summary>Common method to save a user.</summary>
        /// <param name="provider">DB provider.</param>
        /// <returns>Stored model of user.</returns>
        private static UserModel SaveANewUser(IDataBaseProvider provider)
        {
            SignUpModel userData = new SignUpModel()
            {
                Login = "Test_user",
                EMail = "test@mail.mail",
                Password = "Test user password"
            };
            provider.AddUser(userData);
            UserModel userModel = provider.GetUserByLoginMail("test@mail.mail");
            return userModel;
        }


        [TestMethod()]
        public void SuccessSaveProjectTest()
        {
            MsSqlFactory factory = new MsSqlFactory();
            IDataBaseProvider provider = factory.CreateDBProvider(Common.Connectionconfig);
            UserModel userModel = null;
            ProjectModel projectModel = null;
            try
            {
                userModel = SaveANewUser(provider);
                projectModel = new ProjectModel(provider)
                {
                    ProjectName = "Test project name 1",
                    Description = "ProjectDescr",
                    UserIds = new List<decimal>() { userModel.Id }
                };
                Assert.IsTrue(projectModel.SaveProject());
                Assert.IsTrue(projectModel.ProjectId > 0);
                Assert.IsNotNull(
                    ((DragonflyEntities)provider.Context).User_Project.FirstOrDefault(
                        u => u.ID_Project == projectModel.ProjectId),
                    "User_project is not saved");
            }
            finally
            {
                if (projectModel != null && projectModel.ProjectId > 0)
                    provider.DeleteProject(projectModel.ProjectId);
                DeleteUserFromDB(provider.Context, userModel?.Login, userModel?.EMail);
            }
        }

        [TestMethod()]
        public void NoSaveProjectWithoutUsersTest()
        {
            MsSqlFactory factory = new MsSqlFactory();
            IDataBaseProvider provider = factory.CreateDBProvider(Common.Connectionconfig);
            DbContext context = provider.Context;
            ProjectModel projectModel = null;
            try
            {
                projectModel = new ProjectModel(provider)
                {
                    ProjectName = "Test project name 1",
                    Description = "ProjectDescr"
                };
                Assert.IsFalse(projectModel.SaveProject());
                Assert.IsTrue(projectModel.ProjectId == 0);
                Assert.IsNull(
                    ((DragonflyEntities)provider.Context).User_Project.FirstOrDefault(
                        u => u.ID_Project == projectModel.ProjectId),
                    "User_project is saved");
            }
            finally
            {
                if (projectModel != null && projectModel.ProjectId > 0)
                    provider.DeleteProject(projectModel.ProjectId);
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

    }
}