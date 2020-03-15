using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inspinia_MVC5.Models;

namespace Inspinia_MVC5.Views.Shared
{
    public class SharedController : Controller
    {
        private ModelDb db = new ModelDb();

        List<Region> _regions = null;

        public SharedController()
        {
            _regions = db.Regions.ToList();
        }

        public ActionResult _Navigation()
        {
            return PartialView(_regions);
        }
    }
}