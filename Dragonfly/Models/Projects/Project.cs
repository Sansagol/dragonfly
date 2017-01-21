using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dragonfly.Models.Projects
{
    /// <summary>The class describe a single project.</summary>
    public class Project
    {
        /// <summary>Id of the project.</summary>
        public int Projectid { get; set; }

        /// <summary>Id of user-owner of the project.</summary>
        public int UserOwnerId { get; set; }

        /// <summary>Name of the project.</summary>
        public string ProjectName { get; set; }
    }
}