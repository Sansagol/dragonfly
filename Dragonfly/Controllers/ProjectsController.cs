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
        // GET: Projects
        public ActionResult Index()
        {
            ViewBag.Logged = Session["UserId"] != null;
            if (Session["UserId"] == null)
            {
            }
            return View();
        }
    }
}