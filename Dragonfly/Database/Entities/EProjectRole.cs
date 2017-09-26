using Dragonfly.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dragonfly.Database.Entities
{
    /// <summary>Class represent the role for the project.</summary>
    public class EProjectRole
    {
        public decimal ID { get; set; }
        public string Name { get; set; }
        /// <summary>Describe the access rules to a project for this role.</summary>
        public ProjectAccessFunction AccessToProject { get; set; }
        public bool IsAdmin { get; set; }        
    }
}