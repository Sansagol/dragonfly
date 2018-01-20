using Dragonfly.Core;
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
        public decimal ProjectId { get; set; }

        public EUser UserDescription { get; set; }

        #region User data
        public string UserLogin { get { return UserDescription.Login ?? ""; } }

        public string UserName { get { return UserDescription.Name ?? ""; } }

        public string UserSurname { get { return UserDescription.Surname ?? ""; } }

        public string UserEMail { get { return UserDescription.EMail ?? ""; } }
        #endregion

        public decimal ProjectRoleId { get; set; }
        public EProjectRole Role { get; set; }

        #region Role in project
        public decimal ID { get { return Role.ID; } }

        public string Name { get { return Role.Name; } }

        /// <summary>Describe the access rules to a project for this role.</summary>
        public ProjectAccessFunction AccessToProject { get { return Role.AccessToProject; } }

        public bool IsAdmin { get { return Role.IsAdmin; } }

        public string RoleTextDescription { get { return Role.GetFunctionsDescriptions(); } }
        #endregion
    }
}