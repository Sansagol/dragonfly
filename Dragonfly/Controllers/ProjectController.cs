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
        /// <summary>Method add a new project.</summary>
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
                        Users = new List<decimal>() { userId }
                    };
                    return View("EditProject", project);
                }
            }
            return View("CreateProject");
        }

        [HttpPost]
        public ActionResult CreateProject(ProjectModel project)
        {
            if (project.SaveProject())
                return RedirectToAction("Index", "Projects");
            else
                return View("CreateProject");
        }
    }
}
