using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dragonfly.Database.Entities
{
    public class EProject
    { 
        /// <summary>Id of the project.</summary>
        public decimal Id { get; set; }

        /// <summary>Ids of users which can something do with the project.</summary>
        public List<decimal> UserIds { get; set; }

        /// <summary>Name of the project.</summary>
        public string ProjectName { get; set; }

        /// <summary>Project description.</summary>
        public string Description { get; set; }

        /// <summary>Get or set a data, when prodect was creted.</summary>
        public DateTime DateCreation { get; set; }

        public EProject()
        {
            UserIds = new List<decimal>();
        }
    }
}