using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Inspinia_MVC5.Controllers
{
    public class PricingController : Controller
    {
        public ActionResult NewPricesView()
        {
            return View();
        }
    }
}