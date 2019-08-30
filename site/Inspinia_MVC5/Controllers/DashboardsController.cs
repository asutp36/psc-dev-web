using Inspinia_MVC5.Filters;
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

        GetCardsOperations_Result GetCardsOperations = new GetCardsOperations_Result();

        static DateTime now = DateTime.Now;

        public ActionResult Dashboard_1()
        {
            return View();
        }

        public ActionResult Dashboard_2()
        {
            ViewBag.Cards = cards;
            ViewBag.Operations = operations;
            ViewBag.Incomes = GetIncomes(endTime : new DateTime(2019,6,6));

            return View();
        }

        public List<GetWashAmounts_Result> GetIncomes(DateTime endTime, string region="", string wash="", DateTime begTime = new DateTime())
        {
            List<GetWashAmounts_Result> resultset = null;

            //            DateTime dtEnd = DateTime.Today;
            //            DateTime dtBeg = dtEnd.AddDays(-1);
            DateTime dtEnd = endTime;
            DateTime dtBeg = dtEnd.AddDays(-7);

                var prmRegionCode = new System.Data.SqlClient.SqlParameter("@p_RegionCode", System.Data.SqlDbType.Int);
                prmRegionCode.Value = 0;

                var prmWashCode = new System.Data.SqlClient.SqlParameter("@p_WashCode", System.Data.SqlDbType.NVarChar);
                prmWashCode.Value = DBNull.Value;

                var prmPostCode = new System.Data.SqlClient.SqlParameter("@p_PostCode", System.Data.SqlDbType.NVarChar);
                prmPostCode.Value = DBNull.Value;

                var prmBeg = new System.Data.SqlClient.SqlParameter("@p_DateBeg", System.Data.SqlDbType.DateTime);
                prmBeg.Value = dtBeg;

                var prmEnd = new System.Data.SqlClient.SqlParameter("@p_DateEnd", System.Data.SqlDbType.DateTime);
                prmEnd.Value = dtEnd;

                var result = db.Database
                    .SqlQuery<GetWashAmounts_Result>("GetWashAmounts @p_RegionCode, @p_WashCode, @p_PostCode, @p_DateBeg, @p_DateEnd", prmRegionCode, prmWashCode, prmPostCode, prmBeg, prmEnd)
                    .ToList();

                resultset = result;

            return resultset;
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