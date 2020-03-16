using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Inspinia_MVC5.Models;

namespace Inspinia_MVC5.Controllers
{
    public class CollectWashController : Controller
    {
        private ModelDb db = new ModelDb();
        List<Region> _regions = null;
        List<Wash> _washes = null;

        public CollectWashController()
        {
            _regions = db.Regions.ToList();
            _washes = db.Washes.ToList();

            ViewBag.Regions = _regions;
            ViewBag.Washes = _washes;
        }

        public ActionResult CollectList(string begTime, string endTime)
        {
            DateTime startDTime;
            if (!DateTime.TryParse(begTime, out startDTime))
                startDTime = DateTime.Today.AddDays(-1);

            DateTime stopDTime;
            if (!DateTime.TryParse(endTime, out stopDTime))
                stopDTime = DateTime.Today.AddSeconds(-1);

            ViewBag.timeOperationBeg = startDTime.ToString("dd.MM.yyyy HH:mm:ss");
            ViewBag.timeOperationEnd = stopDTime.ToString("dd.MM.yyyy HH:mm:ss");

            return View();
        }

        public ActionResult UpdateViewBagCollectList(string begTime, string endTime, string region, string wash)
        {
            List<GetCollectWashList_Result> viewList = GetCollectList(begTime, endTime, region, wash);

            return PartialView("_CollectList", viewList);
        }

        private List<GetCollectWashList_Result> GetCollectList(string begTime, string endTime, string region, string wash)
        {
            List<GetCollectWashList_Result> resultset = null;

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
                .SqlQuery<GetCollectWashList_Result>("GetCollectWashList @p_RegionCode, @p_WashCode, @p_PostCode, @p_DateBeg, @p_DateEnd", prmRegionCode, prmWashCode, prmPostCode, prmBeg, prmEnd)
                .ToList();

            resultset = result;

            return resultset;
        }

        public ActionResult CollectLipetskView()
        {
            return View("CollectLipetskView");
        }

        public ActionResult _CollectLipetskList(string date)
        {
            List<GetBoxAndCollect_Result> view = GetCollectLipetsk(date);

            return PartialView("_CollectLipetskList", view);
        }

        public ActionResult CollectLipetskFilter(string date)
        {
            List<GetBoxAndCollect_Result> view = GetCollectLipetsk(date);

            return PartialView("_CollectLipetskList", view);
        }

        public List<GetBoxAndCollect_Result> GetCollectLipetsk(string date)
        {
            List<GetBoxAndCollect_Result> resultlist = null;

            DateTime bdate;
            if (!DateTime.TryParse(date, out bdate))
                bdate = DateTime.Today;

            DateTime edate = bdate.AddDays(1);

            edate = edate.AddSeconds(-1);

            var prmBegDate = new System.Data.SqlClient.SqlParameter("@p_DateBeg", System.Data.SqlDbType.DateTime);
            prmBegDate.Value = bdate;

            var prmEndDate = new System.Data.SqlClient.SqlParameter("@p_DateEnd", System.Data.SqlDbType.DateTime);
            prmEndDate.Value = edate;

            var result = db.Database.SqlQuery<GetBoxAndCollect_Result>
                ("GetBoxAndCollect @p_DateBeg, @p_DateEnd",
                prmBegDate, prmEndDate).ToList();

            resultlist = result;

            return resultlist;
        }
    }
}