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

        public void SaveNewPrices(int pr1, int pr2, int pr3, int pr4, int pr5, int pr6, int pr7, int pr8, List<int> posts)
        {
            List<int> prices = new List<int>();

            prices.Add(pr1);
            prices.Add(pr2);
            prices.Add(pr3);
            prices.Add(pr4);
            prices.Add(pr5);
            prices.Add(pr6);
            prices.Add(pr7);
            prices.Add(pr8);

            //отправка запроса на изменение цен на каждом посте
            //return View("NewPricesView", _regions);
        }

        public ActionResult NewPricesView()
        {
            return View(_regions);
        }
    }
}