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
    /// <summary>
    /// Controller is responsoble for the build a projects data
    /// for the user.
    /// </summary>
    public class ProjectsController : Controller
    {
        private string _InitializationError = null;

        private IUserStateManager _UserStateManager = null;

        public ProjectsController()
        {
            _UserStateManager = BaseBindings.UsrStateManager;
        }

        // GET: Projects
        public ActionResult Index()
        {
            if (_UserStateManager.CheckUserAccess(Request))
            {
                ViewBag.Logged = true;
                ProjectsModel model = new ProjectsModel();
                using (IDataBaseProvider provider = BaseBindings.GetNewBaseDbProvider())
                {
                    var projects = provider.GetProjects(0, 10);
                    model.AvailableProjects = projects;
                }
                return View(model);
            }
            else
            {
                ViewBag.Logged = false;
            }
            return View();
        }
    }
}