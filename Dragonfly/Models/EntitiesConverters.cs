using Dragonfly.Database.Entities;
using Dragonfly.Models.UserRoleSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dragonfly.Models
{
    public static class EntitiesConverters
    {
        public static UserModel ToUserModel(this EUserProject usProj)
        {
            UserModel model = new UserModel()
            {
                EMail = usProj.UserEMail,
                Id = usProj.UserId,
                Login = usProj.UserLogin,
                Name = usProj.UserName,
                Surname = usProj.UserSurname                
            };
            return model;
        }

        public static UserRoleModel ToUserProjectModel(this EUserProject usProj)
        {
            UserRoleModel model = new UserRoleModel()
            {
                EMail = usProj.UserEMail,
                Id = usProj.UserId,
                Login = usProj.UserLogin,
                Name = usProj.UserName,
                Surname = usProj.UserSurname,
                IsAdmin = usProj.IsAdmin,
                RoleId = usProj.Role.ID,
                RolsDescription = usProj.RoleTextDescription
            };
            return model;
        }

    }
}