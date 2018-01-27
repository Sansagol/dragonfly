using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dragonfly.Models.UserRoleSystem
{
    public class UserRoleModel: UserModel
    {
        public bool IsAdmin { get; set; }

        public decimal RoleId { get; set; }
        public string RolsDescription { get; set; }
    }
}