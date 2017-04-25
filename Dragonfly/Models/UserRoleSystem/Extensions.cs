using Dragonfly.Database.Entities;
using Dragonfly.Database.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dragonfly.Models.UserRoleSystem
{
    public static class Extensions
    {
        public static UserModel ToUserModel(this EUser srcUser, IUserAccessProvider userProvider)
        {
            if (srcUser == null)
                return null;
            UserModel model = new UserModel(userProvider)
            {
                Id = srcUser.Id,
                Login = srcUser.Login,
                EMail = srcUser.EMail,
                Name = srcUser.Name
            };
            return model;
        }
    }
}