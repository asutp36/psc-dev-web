using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inspinia_MVC5.Models;

namespace Inspinia_MVC5.Controllers
{
    public class PricingController : Controller
    {
        private ModelDb db = new ModelDb();

        List<Region> _regions = null;

        public PricingController()
        {
            _regions = db.Regions.ToList();
        }

        public ActionResult NewPricesView()
        {
            return View(_regions);
        }
    }
}