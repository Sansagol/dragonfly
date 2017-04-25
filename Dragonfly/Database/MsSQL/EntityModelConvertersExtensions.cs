using Dragonfly.Database.Entities;
using Dragonfly.Database.Providers;
using Dragonfly.Models;
using Dragonfly.Models.Clients;
using Dragonfly.Models.Projects;
using Dragonfly.Models.UserRoleSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dragonfly.Database.MsSQL
{
    internal static class EntityModelConvertersExtensions
    {
        /// <summary>
        /// Convert a project from DB oject into model project.
        /// </summary>
        /// <param name="project">Project object which need to convert.</param>
        /// <param name="provider">Provider of database.</param>
        /// <returns>Created project model.</returns>
        /// <exception cref="ArgumentNullException">Empty provider was set.</exception>
        public static EProject ToEProject(this Project project)
        {

            EProject projMod = new EProject()
            {
                Id = project.ID_Project,
                ProjectName = project.Name,
                Description = project.Description,
                DateCreation = project.Date_Create,
            };
            foreach (var up in project.User_Project)
            {
                projMod.UserIds.Add(up.ID_User);
                projMod.Users.Add(up.User.ToEUser());//.ToUserModel(provider));
            }

            return projMod;
        }

        //public static UserModel ToUserModel(this User user, IDataBaseProvider provider)
        //{
        //    if (provider == null)
        //        throw new ArgumentNullException(nameof(provider));
        //    UserModel model = new UserModel()
        //    {
        //        Id = user.ID_User,
        //        EMail = user.E_mail,
        //        Login = user.Login,
        //        Name = user.Name
        //    };
        //    return model;
        //}

        public static ClientType ToClientType(this Client_Type type)
        {
            ClientType tp = new ClientType()
            {
                ID = type.ID_Client_Type,
                 TypeName = type.Type_Name                
            };
            return tp;
        }

        public static ClientModel ToClientModel(this Client client)
        {
            ClientModel model = new ClientModel()
            {
                ID = client.ID_Client,
                Name = client.Name,
                INN = client.INN,
                OGRN = client.OGRN,
                KPP = client.KPP,
                InnerName = client.Inner_Name,
                Type = client.Client_Type.ToClientType()
            };
            return model;
        }

        public static EUser ToEUser(this User src)
        {
            if (src == null)
                return null;
            EUser user = new EUser()
            {
                Id = src.ID_User,
                DateOfCreation = src.Date_Creation,
                EMail = src.E_mail,
                Login = src.Login,
                Name = src.Name,
                Surname = src.Surname,
                IsLdapUser = src.Is_Ldap_User,
                IsBlocked = false
            };
            return user;
        }
    }
}