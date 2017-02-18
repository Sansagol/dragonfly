using Dragonfly.Core;
using Dragonfly.Database;
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
        private IDataBaseProvider _DbProvider = null;
        private string _InitializationError = null;

        public ProjectController()
        {
            try
            {
                _DbProvider = BaseBindings.GetNewDbProvider();
            }
            catch (Exception ex)
            {
                _InitializationError = ex.ToString();
            }
        }

        /// <summary>Method run generatig page for add a new project.</summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddProject()
        {
            ViewBag.Logged = Session["UserId"] != null;
            if (Session["UserId"] != null)
            {
                int userId = -1;
                if (int.TryParse(Session["UserId"].ToString(), out userId))
                {
                    ProjectModel project = new ProjectModel()
                    {
                        UserIds = new List<decimal>() { userId }
                    };
                    return View("CreateProject", project);
                }
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
            if (Session["UserId"] != null)
            {
                project.DbProvider = _DbProvider;
                decimal userId = Convert.ToDecimal(Session["UserId"].ToString());
                project.UserIds = new List<decimal>() { userId };
                if (project.SaveProject())
                    return RedirectToAction("Index", "Projects");
            }
            return View("CreateProject");
        }
    }
}
