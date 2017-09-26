using Dragonfly.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dragonfly.Database.MsSQL.Converters
{
    static class UsersEntityConverters
    {
        public static EUser ToEUser(this User dbUser)
        {
            if (dbUser == null)
                return null;
            EUser user = new EUser()
            {
                Id = dbUser.ID_User,
                DateOfCreation = dbUser.Date_Creation,
                EMail = dbUser.E_mail,
                Login = dbUser.Login,
                Name = dbUser.Name,
                Surname = dbUser.Surname,
                IsLdapUser = dbUser.Is_Ldap_User,
                IsBlocked = false
            };
            return user;
        }
    }
}