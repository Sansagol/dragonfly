using Dragonfly.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dragonfly.Database.Entities
{
    public class EUserProject
    {
        public decimal UserId { get; set; }
        public EUser UserDescription { get; set; }
        public decimal ProjectId { get; set; }
        
        public decimal ProjectRoleId { get; set; }
        public EProjectRole Role { get; set; }
    }
}