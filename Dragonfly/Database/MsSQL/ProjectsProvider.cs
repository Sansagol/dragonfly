using Dragonfly.Database.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dragonfly.Models.Projects;
using Dragonfly.Database.MsSQL.LowLevel;
using Dragonfly.Core.Settings;
using Dragonfly.Database.MsSQL.Converters;
using Dragonfly.Database.Entities;
using Dragonfly.Core;

namespace Dragonfly.Database.MsSQL
{
    class ProjectsProvider : DataProvider, IProjectsProvider
    {
        public ProjectsProvider(IDBContextGenerator contextgenerator) :
            base(contextgenerator)
        {
        }

        public EProject GetProject(decimal projectId)
        {
            try
            {
                using (var context = _ContextGenerator.GenerateContext())
                {
                    var rawProject = (from p in context.Project
                                      where p.ID_Project == projectId
                                      select p).FirstOrDefault();
                    if (rawProject == null)
                        throw new ArgumentException("Target project is not found.");
                    EProject model = rawProject?.ToProjectModel();
                    return model;
                }
            }
            catch (Exception ex)
            {//TODO log
                throw new InvalidOperationException("Error on the project retrieving.", ex);
            }
        }

        public List<EUserProject> GetUsersForProject(decimal projectId)
        {
            if (projectId < 1)
                throw new ArgumentException("Project id must be grather than 0");
            List<EUserProject> users = new List<EUserProject>();
            using (var context = _ContextGenerator.GenerateContext())
            {
                var dbProjUsers = (from p in context.Project
                                   where p.ID_Project == projectId
                                   select p.User_Project).FirstOrDefault();
                if (dbProjUsers != null)
                {
                    foreach (var dbUser in dbProjUsers)
                    {
                        EUserProject userP = FillUserFromDbUserProject(projectId, dbUser);
                        users.Add(userP);
                    }
                }
            }
            return users;
        }

        private EUserProject FillUserFromDbUserProject(decimal projectId, User_Project dbUser)
        {
            EUserProject userP = new EUserProject()
            {
                ProjectId = projectId,
                UserId = dbUser.ID_User,
                ProjectRoleId = dbUser.ID_Project_Role,
            };
            userP.Role = new EProjectRole()
            {
                ID = dbUser.Project_Role.ID_Project_Role,
                IsAdmin = dbUser.Project_Role.Is_Admin,
                Name = dbUser.Project_Role.Role_Name,
                AccessToProject =
                    (ProjectAccessFunction)dbUser.Project_Role.Role_Access_Functions
            };
            userP.UserDescription = dbUser.User.ToEUser();
            return userP;
        }
    }
}