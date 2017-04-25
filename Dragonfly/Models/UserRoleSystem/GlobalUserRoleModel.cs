using Dragonfly.Core.UserAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dragonfly.Models.UserRoleSystem
{
    /// <summary>The class represent the model for user role.</summary>
    public class GlobalUserRoleModel
    {
        /// <summary>Id of the role.</summary>
        public decimal ID { get; set; }

        /// <summary>Name of the role.</summary>
        public decimal Name { get; set; }

        public List<AccessFunction> AccessFunctions { get; set; }


        public GlobalUserRoleModel()
        {
            AccessFunctions = new List<AccessFunction>();
        }
    }
}