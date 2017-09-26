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

        public EUser UserDetails { get; set; }

        public UserModel(IUserAccessProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            _UserAccessDBProvider = provider;
        }

        public EUser GetUserByEmailLogin(string userLogin)
        {
            EUser rawUser = _UserAccessDBProvider.GetUserByLoginMail(userLogin);
            return rawUser;
        }
    }
}