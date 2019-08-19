using Inspinia_MVC5.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Inspinia_MVC5.Models;
using System.Web.Mvc;

namespace Inspinia_MVC5.Controllers
{
    public class DashboardsController : HomeController
    {
        static ContextDb db = new ContextDb();

        IEnumerable<Card> cards = db.Cards;
        IEnumerable<Operation> operations = db.Operations;

        public ActionResult Dashboard_1()
        {
            return View();
        }

        public ActionResult Dashboard_2()
        {
            ViewBag.Cards = cards;
            ViewBag.Operations = operations;

            return View();
        }

        public ActionResult Dashboard_3()
        {
            return View();
        }
        
        public ActionResult Dashboard_4()
        {
            return View();
        }
        
        public ActionResult Dashboard_4_1()
        {
            return View();
        }

        public ActionResult Dashboard_5()
        {
            return View();
        }
    }
}