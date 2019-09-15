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

            ViewBag.Regions = _regions;
            ViewBag.Washes = _washes;
        }

        public ActionResult IncashWashes(string begTime, string endTime, string region, string wash)
        {
            //DateTime beg = new DateTime(2019, 9, 1, 0, 0, 0);
            //DateTime end = new DateTime(2019, 9, 2, 0, 0, 0);

            ////DateTime beg = DateTime.Today.AddDays(-1);
            ////DateTime end = DateTime.Today.AddSeconds(-1);

            //string startDTime = beg.ToString();
            //string stopDTime = end.ToString();

            DateTime startDTime;
            if (!DateTime.TryParse(begTime, out startDTime))
                startDTime = DateTime.Today.AddDays(-1);

            DateTime stopDTime;
            if (!DateTime.TryParse(endTime, out stopDTime))
                stopDTime = DateTime.Today.AddSeconds(-1);

            ViewBag.startDTime = startDTime.ToString("dd.MM.yyyy HH:mm:ss");
            ViewBag.stopDTime = stopDTime.ToString("dd.MM.yyyy HH:mm:ss");
            ViewBag.region = region;
            ViewBag.wash = wash;

            //            ViewBag.Incomes = GetIncomesFromDB(startDTime, stopDTime, "", "");

            return View();
        }

        private List<GetWashAmounts_Result> GetIncomesFromDB(string begTime, string endTime, string region, string wash)
        {
            List<GetWashAmounts_Result> resultset = null;

            DateTime startDTime;
            if (!DateTime.TryParse(begTime, out startDTime))
                startDTime = DateTime.Today.AddDays(-1);

            DateTime stopDTime;
            if (!DateTime.TryParse(endTime, out stopDTime))
                stopDTime = DateTime.Today.AddSeconds(-1);

            Int32 regionnum;
            if (!Int32.TryParse(region, out regionnum))
                regionnum = 0;

            var prmRegionCode = new System.Data.SqlClient.SqlParameter("@p_RegionCode", System.Data.SqlDbType.Int);
            prmRegionCode.Value = regionnum;

            var prmWashCode = new System.Data.SqlClient.SqlParameter("@p_WashCode", System.Data.SqlDbType.NVarChar);
            if (wash != null)
                prmWashCode.Value = wash;
            else
                prmWashCode.Value = DBNull.Value;

            var prmPostCode = new System.Data.SqlClient.SqlParameter("@p_PostCode", System.Data.SqlDbType.NVarChar);
            prmPostCode.Value = DBNull.Value;

            var prmBeg = new System.Data.SqlClient.SqlParameter("@p_DateBeg", System.Data.SqlDbType.DateTime);
            prmBeg.Value = startDTime;

            var prmEnd = new System.Data.SqlClient.SqlParameter("@p_DateEnd", System.Data.SqlDbType.DateTime);
            prmEnd.Value = stopDTime;

            var result = db.Database
                .SqlQuery<GetWashAmounts_Result>("GetWashAmounts @p_RegionCode, @p_WashCode, @p_PostCode, @p_DateBeg, @p_DateEnd", prmRegionCode, prmWashCode, prmPostCode, prmBeg, prmEnd)
                .ToList();

            resultset = result;

            return resultset;
        }

        //        public ActionResult FilterIncashes(DateTime begTime, DateTime endTime, string region, string wash)

        // GET: /FilterIncashes/
        public ActionResult FilterIncashes(string begTime, string endTime, string region, string wash)
        {
            List<GetWashAmounts_Result> viewList = GetIncomesFromDB(begTime, endTime, region, wash);

            return PartialView("_IncashWashesList", viewList);
        }
    }
}