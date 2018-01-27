using Dragonfly.Core;
using Dragonfly.Core.UserAccess;
using Dragonfly.Database;
using Dragonfly.Database.Entities;
using Dragonfly.Database.Providers;
using Dragonfly.Models.Projects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dragonfly.Controllers
{
    [ControllersException]
    public class ProjectController : Controller
    {
        private string _InitializationError = null;

        private IUserAuthenticateStateManager _UserStateManager = null;
        private IProjectsProvider _ProjectsProvider;

        public ProjectController()
        {
            _UserStateManager = BaseBindings.UsrStateManager;
            _ProjectsProvider = BaseBindings.DBFactory.CreateProjectsProvider();
        }

        /// <summary>Method run generatig page for add a new project.</summary>
        /// <returns></returns>
        [HttpGet]
        [ControllersException]
        public ActionResult AddProject()
        {
            ViewBag.Logged = _UserStateManager.CheckUserAccess(Request, Response);
            decimal userId = BaseBindings.CookiesManager.GetCookieValueDecimal(Request, CookieType.UserId);
            ProjectModel project = new ProjectModel();
            //TODO change the role id to the loaded from the DB
            project.AddUserToProject(userId, 0);
            ViewBag.IsNewProject = true;
            return View("CreateProject", project);
        }

        [HttpPost]
        [ControllersException]
        public ActionResult SaveProject(ProjectModel project)
        {
            _UserStateManager.CheckUserAccess(Request, Response);
            if (project.Id <= 0)
            {
                decimal userId = BaseBindings.CookiesManager.GetCookieValueDecimal(Request, CookieType.UserId);
                project.AddUserToProject(userId, 0);
            }
            //TODO change the role id to the loaded from the DB
            if (project.SaveProject())
                return RedirectToAction("Index", "Projects");
            return View("CreateProject");
        }

        [HttpGet]
        [ControllersException]
        public ActionResult Index(decimal projectId)
        {
            ProjectModel model = null;
            ViewBag.Logged = _UserStateManager.CheckUserAccess(Request, Response);
            var provider = BaseBindings.DBFactory.CreateProjectsProvider();
            try
            {
                model = new ProjectModel(provider.GetProject(projectId));
            }
            catch (Exception ex)
            {
                model = new ProjectModel
                {
                    ProjectName = "<Project is not found>",
                };
                ViewBag.Error = ex.GetFullMessage();
            }
            return View("Index", model);
        }

        [HttpGet]
        [ControllersException]
        public ActionResult Edit(decimal id)
        {
            ViewBag.Logged = _UserStateManager.CheckUserAccess(Request, Response);
            var project = _ProjectsProvider.GetProject(id);
            ProjectModel model = new ProjectModel(project);
            ViewBag.IsNewProject = false;
            return View("CreateProject", model);
        }
    }
}
