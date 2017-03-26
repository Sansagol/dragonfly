using Dragonfly.Core;
using Dragonfly.Database;
using Dragonfly.Database.Providers;
using Dragonfly.Models.Projects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dragonfly.Controllers
{
    public class ProjectController : Controller
    {
        private string _InitializationError = null;

        private IUserStateManager _UserStateManager = null;

        public ProjectController()
        {
            _UserStateManager = BaseBindings.UsrStateManager;
        }

        /// <summary>Method run generatig page for add a new project.</summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddProject()
        {
            if (_UserStateManager.CheckUserAccess(Request))
            {
                decimal userId = BaseBindings.CookiesManager.GetCookieValueDecimal(Request, CookieType.UserId);
                ProjectModel project = new ProjectModel()
                {
                    UserIds = new List<decimal>() { userId }
                };
                ViewBag.Logged = true;
                return View("CreateProject", project);
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(_InitializationError))
                    ViewBag.InitError = _InitializationError;
                else
                    ViewBag.Error = "User not logged";
            }
            return View("CreateProject");
        }

        [HttpPost]
        public ActionResult CreateProject(ProjectModel project)
        {
            if (_UserStateManager.CheckUserAccess(Request))
            {
                decimal userId = BaseBindings.CookiesManager.GetCookieValueDecimal(Request, CookieType.UserId);
                using (IDataBaseProvider provider = BaseBindings.GetNewBaseDbProvider())
                {
                    project.DbProvider = provider;
                    project.UserIds = new List<decimal>() { userId };
                    if (project.SaveProject())
                        return RedirectToAction("Index", "Projects");
                }
            }
            return View("CreateProject");
        }
    }
}
