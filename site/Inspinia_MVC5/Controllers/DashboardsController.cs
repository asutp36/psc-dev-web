﻿using Inspinia_MVC5.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inspinia_MVC5.Models;

namespace Inspinia_MVC5.Controllers
{
    public class DashboardsController : HomeController
    {
        static ModelDb db = new ModelDb();

        IEnumerable<Card> cards = db.Cards;
        IEnumerable<Operation> operations = db.Operations;

        static ModelIncomes dbIncomes = new ModelIncomes();

        IEnumerable<Income> incomes = dbIncomes.Incomes;

        GetCardsOperations_Result GetCardsOperations = new GetCardsOperations_Result();

        public ActionResult Dashboard_1()
        {
            return View();
        }

        public ActionResult Dashboard_2()
        {
            ViewBag.Cards = cards;
            ViewBag.Operations = operations;
            ViewBag.Incomes = incomes;

            return View();
        }

        [HttpGet]
        [ActionName("UpdateTable")]
        public ActionResult UpdateTable()
        {
            ViewBag.Operations = operations;
            
            return PartialView("~/Views/Dashboards/UpdateTable.cshtml");
        }

        //[HttpGet]
        //[ActionName("Filter")]
        //public ActionResult Filter()
        //{
        //    ViewBag.Operations = GetCardsOperations;

        //    return ViewBag();
        //}

        [HttpGet]
        [ActionName("UpdateTime")]
        public string UpdateTime()
        {
            return "Обновлено в " + DateTime.Now.ToString();
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