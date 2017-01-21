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
        /// <summary>Methot to add a new project.</summary>
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
                    ProjectModel project = new ProjectModel() { UserOwnerId = userId };
                    return View("EditProject", project);
                }
            }
            return View("EditProject");
        }
    }
}
