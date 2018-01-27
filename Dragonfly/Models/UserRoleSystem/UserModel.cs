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
    public class UserModel : ModelBase
    {
        private IUserAccessProvider _UserAccessDBProvider = null;

        [Obsolete]
        public EUser UserDetails { get; set; }

        public decimal Id { get; set; }

        public string Login { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string EMail { get; set; }

        /// <summary>Is it LDAP user or it was registered manually.</summary>
        public bool IsLdapUser { get; set; }

        public DateTime DateOfCreation { get; set; }

        /// <summary>Reserved field.</summary>
        public bool IsBlocked { get; set; }

        public UserModel()
        {
            LoadRoles();
        }

        [Obsolete]
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

        #region loader
        private void LoadRoles()
        {

        }
        #endregion
    }
}