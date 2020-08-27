using Inspinia_MVC5.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inspinia_MVC5.Models;
using System.Security.Cryptography.Xml;

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
            ViewBag.Cards = db.Cards;

            ViewBag.Cards = cards;
            ViewBag.Operations = operations;

            //GetIncomesFromDBToday();

            ViewBag.Regions = db.Regions;
            ViewBag.Washes = db.Washes;

            return View();
        }

        public ActionResult _IncreasesFromStart(string enddate)
        {
            int increases = GetIncreasesFromStart(enddate, "", "");

            return PartialView("_NumLabel", increases);
        }

        public int GetIncreasesFromStart(string enddate, string login, string washcode)
        {
            DateTime edate;
            if (!DateTime.TryParse(enddate, out edate))
                edate = DateTime.Now;

            var prmEndDate = new System.Data.SqlClient.SqlParameter("@p_DateEnd", System.Data.SqlDbType.DateTime);
            prmEndDate.Value = edate;

            var prmLogin = new System.Data.SqlClient.SqlParameter("@p_Login", System.Data.SqlDbType.NVarChar);
            if (login == null)
            {
                login = "";
            }
            prmLogin.Value = login;

            var prmWashCode = new System.Data.SqlClient.SqlParameter("@p_WashCode", System.Data.SqlDbType.NVarChar);
            if (washcode == null)
            {
                washcode = "";
            }
            prmWashCode.Value = washcode;

            //int result = db.GetIncreaseBefDate(edate, login, washcode);
            int result = db.Database.ExecuteSqlCommand(
                "GetIncreaseBefDate @p_DateEnd, @p_Login, @p_WashCode ",
                prmEndDate, prmLogin, prmWashCode);

            return result;
        }

        public ActionResult _IncreasesYesterday(string begdate, string enddate)
        {
            int increases = GetIncreasesYesterday(begdate, enddate, "", "");

            return PartialView("_NumLabel", increases);
        }

        public int GetIncreasesYesterday(string begdate, string enddate, string login, string washcode)
        {
            DateTime bdate;
            if (!DateTime.TryParse(begdate, out bdate))
                bdate = DateTime.Today.AddDays(-1);

            DateTime edate;
            if (!DateTime.TryParse(enddate, out edate))
                edate = DateTime.Today.AddSeconds(-1);

            var prmBegDate = new System.Data.SqlClient.SqlParameter("@p_DateBeg", System.Data.SqlDbType.DateTime);
            prmBegDate.Value = bdate;

            var prmEndDate = new System.Data.SqlClient.SqlParameter("@p_DateEnd", System.Data.SqlDbType.DateTime);
            prmEndDate.Value = edate;

            var prmLogin = new System.Data.SqlClient.SqlParameter("@p_Login", System.Data.SqlDbType.NVarChar);
            if (login == null)
            {
                login = "";
            }
            prmLogin.Value = login;

            var prmWashCode = new System.Data.SqlClient.SqlParameter("@p_WashCode", System.Data.SqlDbType.NVarChar);
            if (washcode == null)
            {
                washcode = "";
            }
            prmWashCode.Value = washcode;

            int result = db.Database.ExecuteSqlCommand(
                "GetIncreaseDurPeriod @p_DateBeg, @p_DateEnd, @p_Login, @p_WashCode ",
                prmBegDate, prmEndDate, prmLogin, prmWashCode);

            return result;
        }

        public int GetIncreasesFromStart(string enddate, string login, string washcode)
        {
            DateTime edate;
            if (!DateTime.TryParse(enddate, out edate))
                edate = DateTime.Now;

            var prmEndDate = new System.Data.SqlClient.SqlParameter("@p_DateEnd", System.Data.SqlDbType.DateTime);
            prmEndDate.Value = edate;

            var prmLogin = new System.Data.SqlClient.SqlParameter("@p_Login", System.Data.SqlDbType.NVarChar);
            if (login == null)
            {
                login = "";
            }
            prmLogin.Value = login;

            var prmWashCode = new System.Data.SqlClient.SqlParameter("@p_WashCode", System.Data.SqlDbType.NVarChar);
            if (washcode == null)
            {
                washcode = "";
            }
            prmWashCode.Value = washcode;

            //int result = db.GetIncreaseBefDate(edate, login, washcode);
            int result = db.Database.ExecuteSqlCommand(
                "GetIncreaseBefDate @p_DateEnd, @p_Login, @p_WashCode ",
                prmEndDate, prmLogin, prmWashCode);

            return result;
        }

        public ActionResult _CollectLastMonth(string begdate, string enddate)
        {
            int collects = GetCollectLastMonth(begdate, enddate, "", "");

            return PartialView("_NumLabel", collects);
        }

        public int GetCollectLastMonth(string begdate, string enddate, string login, string washcode)
        {
            DateTime bdate;
            if (!DateTime.TryParse(begdate, out bdate))
                bdate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1);

            DateTime edate;
            if (!DateTime.TryParse(enddate, out edate))
                edate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddSeconds(-1);

            var prmBegDate = new System.Data.SqlClient.SqlParameter("@p_DateBeg", System.Data.SqlDbType.DateTime);
            prmBegDate.Value = bdate;

            var prmEndDate = new System.Data.SqlClient.SqlParameter("@p_DateEnd", System.Data.SqlDbType.DateTime);
            prmEndDate.Value = edate;

            var prmLogin = new System.Data.SqlClient.SqlParameter("@p_Login", System.Data.SqlDbType.NVarChar);
            if (login == null)
            {
                login = "";
            }
            prmLogin.Value = login;

            var prmWashCode = new System.Data.SqlClient.SqlParameter("@p_WashCode", System.Data.SqlDbType.NVarChar);
            if (washcode == null)
            {
                washcode = "";
            }
            prmWashCode.Value = washcode;

            int result = db.Database.ExecuteSqlCommand(
                "GetCollectDurPeriod @p_DateBeg, @p_DateEnd, @p_Login, @p_WashCode ",
                prmBegDate, prmEndDate, prmLogin, prmWashCode);

            return result;
        }
        private void GetIncomesFromDBToday()
        {
            ViewBag.Incomes = GetIncomesFromDB(DateTime.Today, DateTime.Now, "", "");
        }

        //public ActionResult GetIncomes()
        //{

        //}

        public List<GetWashAmounts_Result> GetIncomesFromDB(DateTime begTime, DateTime endTime, string region, string wash)
        {
            List<GetWashAmounts_Result> resultset = null;

            //            DateTime dtEnd = DateTime.Today;
            //            DateTime dtBeg = dtEnd.AddDays(-1);

            var prmRegionCode = new System.Data.SqlClient.SqlParameter("@p_RegionCode", System.Data.SqlDbType.Int);
            prmRegionCode.Value = 0;

            Int32 intval;
            if (Int32.TryParse(region, out intval))
                prmRegionCode.Value = intval;

            var prmWashCode = new System.Data.SqlClient.SqlParameter("@p_WashCode", System.Data.SqlDbType.NVarChar);
            prmWashCode.Value = wash;

            var prmPostCode = new System.Data.SqlClient.SqlParameter("@p_PostCode", System.Data.SqlDbType.NVarChar);
            prmPostCode.Value = DBNull.Value;

            var prmBeg = new System.Data.SqlClient.SqlParameter("@p_DateBeg", System.Data.SqlDbType.DateTime);
            prmBeg.Value = begTime;

            var prmEnd = new System.Data.SqlClient.SqlParameter("@p_DateEnd", System.Data.SqlDbType.DateTime);
            prmEnd.Value = endTime;

            var result = db.Database
                .SqlQuery<GetWashAmounts_Result>("GetWashAmounts @p_RegionCode, @p_WashCode, @p_PostCode, @p_DateBeg, @p_DateEnd", prmRegionCode, prmWashCode, prmPostCode, prmBeg, prmEnd)
                .ToList();

            resultset = result;

            return resultset;
        }

        //[HttpGet]
        //[ActionName("UpdateTable")]
        //public ActionResult UpdateTable()
        //{
        //    ViewBag.Operations = operations;
            
        //    return PartialView("~/Views/Dashboards/UpdateTable.cshtml");
        //}

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