using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inspinia_MVC5.Models;

namespace Inspinia_MVC5.Controllers
{
    public class MonitoringController : Controller
    {
        private ModelDb db = new ModelDb();

        List<Company> _companies = null;
        List<Post> _posts = null;

        Post post = null;

        public MonitoringController()
        {
            _posts = db.Posts.ToList();
            _companies = db.Companies.ToList();
        }

        public ActionResult MonitoringView()
        {
            return View(_companies);
        }

        public ActionResult PostMonitoringView(int IDPost)
        {
            post = _posts.Find(x => x.IDPost == IDPost);

            return PartialView("PostMonitoringView", post);
        }
    }
}