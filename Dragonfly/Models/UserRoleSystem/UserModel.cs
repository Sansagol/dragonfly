using Dragonfly.Database.Entities;
using Dragonfly.Database.Providers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Dragonfly.Models.UserRoleSystem
{
    /// <summary>The model represent a user in the system.</summary>
    public class UserModel
    {
        private IUserAccessProvider _UserAccessDBProvider = null;

        public string Login { get; set; }
        public string Name { get; set; }
        public string EMail { get; set; }

        public decimal Id { get; set; }

        /// <summary>The roles assigned to the user.</summary>
        List<GlobalUserRoleModel> Roles { get; set; }

        public UserModel(IUserAccessProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            _UserAccessDBProvider = provider;

            Roles = new List<GlobalUserRoleModel>();
        }

        public UserModel GetUserById(decimal userId)
        {
            EUser rawUser = _UserAccessDBProvider.GetUserById(userId);
            return rawUser?.ToUserModel(_UserAccessDBProvider);
        }

        public UserModel GetUserByEmailLogin(string userLogin)
        {
            EUser rawUser = _UserAccessDBProvider.GetUserByLoginMail(userLogin);
            return rawUser?.ToUserModel(_UserAccessDBProvider);
        }
    }
}