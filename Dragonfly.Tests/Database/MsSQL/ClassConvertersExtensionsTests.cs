using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dragonfly.Database.MsSQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dragonfly.Models.Projects;
using System.Data.Entity;
using Dragonfly.Tests;
using Dragonfly.Models;
using Dragonfly.Models.UserRoleSystem;

namespace Dragonfly.Database.MsSQL.Tests
{
    [TestClass()]
    public class ClassConvertersExtensionsTests
    {
        [TestMethod()]
        public void ToProjectModelTest()
        {
            MsSqlFactory factory = new MsSqlFactory();
            MsSqlDataProvider provider = factory.CreateDBProvider(Common.Connectionconfig) as MsSqlDataProvider;

            Project proj = InitializeProject();

            ProjectModel model = proj.ToProjectModel(provider);

            Assert.AreEqual(proj.ID_Project, model.ProjectId, "Bad project id");
            Assert.AreEqual(proj.Name, model.ProjectName, "Bad name");
            Assert.AreEqual(proj.Date_Create, model.DateCreation, "Bad date of creation");
            Assert.AreEqual(proj.Description, model.Description, "Bad description");
            CollectionAssert.AreEqual(
                proj.User_Project.Select(u => u.ID_User).ToList(),
                model.UserIds,
                "Users ids are not equal");

            CollectionAssert.AreEqual(
                proj.User_Project.Select(u => u.User.ID_User).ToList(),
                model.Users.Select(u => u.Id).ToList(),
                "Users are not equal");
        }

        private static Project InitializeProject()
        {
            Project proj = new Project()
            {
                Date_Create = new DateTime(1970, 1, 1),
                Description = "Descr",
                ID_Project = 1,
                Name = "Project name",
            };
            User usr = InitUser();
            proj.User_Project = new List<User_Project>()
            {
                new User_Project() {
                    ID_User = usr.ID_User,
                    ID_Project = 1,
                    ID_Project_Role =2,
                    User = usr
                }
            };
            return proj;
        }

        private static User InitUser()
        {
            return new User()
            {
                ID_User = 11,
                E_mail = "User@mail.m",
                Login = "User login",
                Name = "Sample name"
            };
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ToProjectModelWithoutProviderTest()
        {
            Project proj = InitializeProject();
            ProjectModel model = proj.ToProjectModel(null);
        }

        [TestMethod()]
        public void UserToUserModelTest()
        {
            MsSqlFactory factory = new MsSqlFactory();
            MsSqlDataProvider provider = factory.CreateDBProvider(
                Common.Connectionconfig) as MsSqlDataProvider;

            User user = InitUser();

            UserModel model = user.ToUserModel(provider);

            Assert.AreEqual(user.ID_User, model.Id, "Bad user id");
            Assert.AreEqual(user.Name, model.Name, "Bad name");
            Assert.AreEqual(user.Login, model.Login, "Bad login");
            Assert.AreEqual(user.E_mail, model.EMail, "Bad e-mail");
        }
    }
}