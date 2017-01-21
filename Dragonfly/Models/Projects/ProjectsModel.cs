using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dragonfly.Models.Projects
{
    /// <summary>The model describe a projects page.</summary>
    public class ProjectsModel
    {
        /// <summary>Id of current user.</summary>
        public int UserId { get; set; }

        /// <summary>All Available projects for a user.</summary>
        public List<ProjectModel> AvailableProjects { get; set; }
    }
}