using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Inspinia_MVC5.Models;

namespace Inspinia_MVC5.Controllers
{
    public class IncashController : Controller
    {
        private ModelDb db = new ModelDb();
        List<Region> _regions = null;
        List<Wash> _washes = null;


        public IncashController()
        {
            _regions = db.Regions.ToList();
            _washes = db.Washes.ToList();
        }

        public ActionResult ProductsGrid()
        {
            return View();
        }

        public ActionResult IncashWashes()
        {
            DateTime startDTime = new DateTime(2019, 6, 20, 0, 0, 0);
            DateTime stopDTime = new DateTime(2019, 6, 21, 0, 0, 0);

            ViewBag.Regions = _regions;
            ViewBag.Washes = _washes;

            ViewBag.Incomes = GetIncomesFromDB(startDTime, stopDTime, "", "");

            return View();
        }

        private List<GetWashAmounts_Result> GetIncomesFromDB(DateTime begTime, DateTime endTime, string region, string wash)
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

//        public ActionResult FilterIncashes(DateTime begTime, DateTime endTime, string region, string wash)

        // POST: Incash/FilterIncashes
        [HttpPost]
        public ActionResult FilterIncashes(string begTime, string endTime, string region, string wash)
        {
            //DateTime startDTime;
            //DateTime.TryParse(begTime, out startDTime);

            //DateTime stopDTime;
            //DateTime.TryParse(endTime, out stopDTime);

            //ViewBag.Incomes = GetIncomesFromDB(startDTime, stopDTime, region, wash);

            return View();
        }
    }
}