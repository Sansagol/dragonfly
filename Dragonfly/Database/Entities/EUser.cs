using Dragonfly.Models.UserRoleSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dragonfly.Database.Entities
{
    /// <summary>Represent a user in a database.</summary>
    public class EUser
    {
        public decimal Id { get; set; }

        public string Login { get; set; }

        public string Name { get; set; }
        public string Surname { get; set; }

        public string EMail { get; set; }

        /// <summary>
        /// Is it LDAP user or it was registered manually.
        /// </summary>
        public bool IsLdapUser { get; set; }

        public DateTime DateOfCreation { get; set; }

        /// <summary>Reserved field.</summary>
        public bool IsBlocked { get; set; }
        
        /// <summary>The roles assigned to the user.</summary>
        public List<GlobalUserRoleModel> Roles { get; private set; }

        public EUser()
        {
            Roles = new List<GlobalUserRoleModel>();
        }
    }
}