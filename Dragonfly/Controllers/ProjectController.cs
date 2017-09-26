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

        public ProjectController()
        {
            _UserStateManager = BaseBindings.UsrStateManager;
        }

        /// <summary>Method run generatig page for add a new project.</summary>
        /// <returns></returns>
        [HttpGet]
        [ControllersException]
        public ActionResult AddProject()
        {
            _UserStateManager.CheckUserAccess(Request, Response);
            decimal userId = BaseBindings.CookiesManager.GetCookieValueDecimal(Request, CookieType.UserId);
            ProjectModel project = new ProjectModel()
            {
                Users = new List<EUserProject>() {
                    new EUserProject { UserId = userId }
                }
            };
            ViewBag.Logged = true;
            return View("CreateProject", project);
        }

        [HttpPost]
        [ControllersException]
        public ActionResult CreateProject(ProjectModel project)
        {
            _UserStateManager.CheckUserAccess(Request, Response);
            decimal userId = BaseBindings.CookiesManager.GetCookieValueDecimal(Request, CookieType.UserId);
            IDataBaseProvider provider = BaseBindings.GetNewBaseDbProvider();
            project.DbProvider = provider;
            project.Users = new List<EUserProject>() {
                    new EUserProject { UserId = userId }
            };
            if (project.SaveProject())
                return RedirectToAction("Index", "Projects");
            return View("CreateProject");
        }

        [HttpGet]
        [ControllersException]
        public ActionResult Index(decimal projectId)
        {
            ProjectModel model = new ProjectModel();
            ViewBag.Logged = _UserStateManager.CheckUserAccess(Request, Response);
            if (ViewBag.Logged)
            {
                var provider = BaseBindings.DBFactory.CreateProjectsProvider();
                try
                {
                    model.ProjectDetails = provider.GetProject(projectId);
                }
                catch (Exception ex)
                {
                    model.ProjectDetails = new EProject
                    {
                        ProjectName = "<Project is not found>",
                    };
                    ViewBag.Error = ex.GetFullMessage();
                }
            }
            return View("Index", model);
        }
    }
}
