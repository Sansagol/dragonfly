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
        public string Login{get;set;}
        public string Name { get; set; }
        public string EMail { get; set; }

        public decimal Id { get; set; }

        /// <summary>The roles assigned to the user.</summary>
        List<GlobalUserRoleModel> Roles { get; set; }
    }
}