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
        List<Post> _posts = null;
        List<Device> _devices = null;

        public CollectWashController()
        {
            _washes = db.Washes.Where(w => w.Code == "М13" || w.Code == "М14").ToList();
            _devices = db.Devices.ToList();

            _regions = new List<Region>();
            _posts = new List<Post>();

            foreach (Wash w in _washes)
            {
                if (!_regions.Contains(w.Region))
                    _regions.Add(w.Region);

                for (int i = w.Posts.Count - 1; i >= 0; i--)
                {
                    if (_devices.Find(d => d.IDDevice == w.Posts.ElementAt(i).IDDevice).IDDeviceType != 2)
                    {
                        w.Posts.Remove(w.Posts.ElementAt(i));
                    }
                }

                _posts.AddRange(w.Posts);
            }

            foreach (var r in _regions)
            {
                for (int i = r.Washes.Count - 1; i >= 0; i--)
                {
                    string code = r.Washes.ElementAt(i).Code;

                    if (code == "М13" || code == "М14")
                    {
                    }
                    else
                    {
                        r.Washes.Remove(r.Washes.ElementAt(i));
                    }
                }
            }

            ViewBag.Regions = _regions;
            ViewBag.Washes = _washes;
            ViewBag.Posts = _posts;
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

        public ActionResult CollectByWashesView()
        {
            DateTime edate = DateTime.Now;
            ViewBag.EndDate = edate;

            return View("CollectByWashesView");
        }

        public ActionResult _CollectByWashesList(string region, string wash, string begdate, string enddate)
        {
            List<GetCollectByWashs_Result> view = GetCollectByWashes(region, wash, begdate, enddate);

            return PartialView("_CollectByWashesList", view);
        }

        public List<GetCollectByWashs_Result> GetCollectByWashes(string region, string wash, string begdate, string enddate)
        {
            List<GetCollectByWashs_Result> resultlist = null;

            DateTime bdate;
            if (!DateTime.TryParse(begdate, out bdate))
                bdate = DateTime.Today.AddYears(-10);

            DateTime edate;
            if (!DateTime.TryParse(enddate, out edate))
                edate = DateTime.Now;

            var prmRegion = new System.Data.SqlClient.SqlParameter("@p_RegionCode", System.Data.SqlDbType.Int);
            if (region == "")
            {
                region = "0";
            }
            prmRegion.Value = Convert.ToInt32(region);

            var prmWash = new System.Data.SqlClient.SqlParameter("@p_WashCode", System.Data.SqlDbType.NVarChar);
            if (wash == null)
            {
                wash = "";
            }
            prmWash.Value = wash;

            var prmBegDate = new System.Data.SqlClient.SqlParameter("@p_DateBeg", System.Data.SqlDbType.DateTime);
            prmBegDate.Value = bdate;

            var prmEndDate = new System.Data.SqlClient.SqlParameter("@p_DateEnd", System.Data.SqlDbType.DateTime);
            prmEndDate.Value = edate;

            var result = db.Database.SqlQuery<GetCollectByWashs_Result>
                ("GetCollectByWashs @p_DateBeg, @p_DateEnd, @p_RegionCode, @p_WashCode ",
                prmBegDate, prmEndDate, prmRegion, prmWash).ToList();

            resultlist = result;

            return resultlist;
        }

        public ActionResult CollectByWashesFilter(string region, string wash, string begdate, string enddate)
        {
            List<GetCollectByWashs_Result> view = GetCollectByWashes(region, wash, begdate, enddate);

            return PartialView("_CollectByWashesList", view);
        }

        public ActionResult CollectByDaysView(string begdate, string enddate, string wash)
        {
            char[] chars = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            int idx = wash.IndexOfAny(chars);
            if (idx > -1)
                wash = 'М' + wash.Substring(idx);

            Wash Wash = _washes.Find(w => w.Code == wash);

            ViewBag.Region = Wash.Region.Code;
            ViewBag.Wash = Wash.Code;

            DateTime bdate;
            if (!DateTime.TryParse(begdate, out bdate))
                bdate = DateTime.Today.AddYears(-10);

            DateTime edate;
            if (!DateTime.TryParse(enddate, out edate))
                edate = DateTime.Now;

            ViewBag.BegDate = begdate;
            ViewBag.EndDate = enddate;

            return View("CollectByDaysView");
        }

        public ActionResult _CollectByDaysList(string region, string wash, string begdate, string enddate)
        {
            List<GetCollectByDays_Result> view = GetCollectByDays(region, wash, begdate, enddate);

            return PartialView("_CollectByDaysList", view);
        }

        public List<GetCollectByDays_Result> GetCollectByDays(string region, string wash, string begdate, string enddate)
        {
            List<GetCollectByDays_Result> resultlist = null;

            DateTime bdate;
            if (!DateTime.TryParse(begdate, out bdate))
                bdate = DateTime.Today.AddYears(-10);

            DateTime edate;
            if (!DateTime.TryParse(enddate, out edate))
                edate = DateTime.Now;

            var prmRegion = new System.Data.SqlClient.SqlParameter("@p_RegionCode", System.Data.SqlDbType.Int);
            if (region == "")
            {
                region = "0";
            }
            prmRegion.Value = Convert.ToInt32(region);

            var prmWash = new System.Data.SqlClient.SqlParameter("@p_WashCode", System.Data.SqlDbType.NVarChar);
            if (wash == null)
            {
                wash = "";
            }
            prmWash.Value = wash;

            var prmBegDate = new System.Data.SqlClient.SqlParameter("@p_DateBeg", System.Data.SqlDbType.DateTime);
            prmBegDate.Value = bdate;

            var prmEndDate = new System.Data.SqlClient.SqlParameter("@p_DateEnd", System.Data.SqlDbType.DateTime);
            prmEndDate.Value = edate;

            var result = db.Database.SqlQuery<GetCollectByDays_Result>
                ("GetCollectByDays @p_DateBeg, @p_DateEnd, @p_RegionCode, @p_WashCode ",
                prmBegDate, prmEndDate, prmRegion, prmWash).ToList();

            resultlist = result;

            return resultlist;
        }

        public ActionResult CollectByDaysFilter(string region, string wash, string begdate, string enddate)
        {
            List<GetCollectByDays_Result> view = GetCollectByDays(region, wash, begdate, enddate);

            return PartialView("_CollectByDaysList", view);
        }

        public ActionResult CollectByPostsView(string begdate, string enddate, string wash)
        {
            char[] chars = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            int idx = wash.IndexOfAny(chars);
            if (idx > -1)
                wash = 'М' + wash.Substring(idx);

            Wash Wash = _washes.Find(w => w.Code == wash);

            ViewBag.Region = Wash.Region.Code;
            ViewBag.Wash = Wash.Code;

            DateTime bdate;
            if (!DateTime.TryParse(begdate, out bdate))
                bdate = DateTime.Today.AddYears(-10);

            DateTime edate;
            if (!DateTime.TryParse(enddate, out edate))
                edate = DateTime.Now;

            ViewBag.BegDate = begdate;
            ViewBag.EndDate = enddate;

            return View("CollectByPostsView");
        }

        public ActionResult _CollectByPostsList(string region, string wash, string post, string begdate, string enddate)
        {
            List<GetCollectByPosts_Result> view = GetCollectByPosts(region, wash, post, begdate, enddate);

            return PartialView("_CollectByPostsList", view);
        }

        public List<GetCollectByPosts_Result> GetCollectByPosts(string region, string wash, string post, string begdate, string enddate)
        {
            List<GetCollectByPosts_Result> resultlist = null;

            DateTime bdate;
            if (!DateTime.TryParse(begdate, out bdate))
                bdate = DateTime.Today.AddYears(-10);

            DateTime edate;
            if (!DateTime.TryParse(enddate, out edate))
                edate = DateTime.Now;

            var prmRegion = new System.Data.SqlClient.SqlParameter("@p_RegionCode", System.Data.SqlDbType.Int);
            if (region == "")
            {
                region = "0";
            }
            prmRegion.Value = Convert.ToInt32(region);

            var prmWash = new System.Data.SqlClient.SqlParameter("@p_WashCode", System.Data.SqlDbType.NVarChar);
            if (wash == null)
            {
                wash = "";
            }
            prmWash.Value = wash;

            var prmPost = new System.Data.SqlClient.SqlParameter("@p_PostCode", System.Data.SqlDbType.NVarChar);
            if (post == null)
            {
                post = "";
            }
            prmPost.Value = post;

            var prmBegDate = new System.Data.SqlClient.SqlParameter("@p_DateBeg", System.Data.SqlDbType.DateTime);
            prmBegDate.Value = bdate;

            var prmEndDate = new System.Data.SqlClient.SqlParameter("@p_DateEnd", System.Data.SqlDbType.DateTime);
            prmEndDate.Value = edate;

            var result = db.Database.SqlQuery<GetCollectByPosts_Result>
                ("GetCollectByPosts @p_DateBeg, @p_DateEnd, @p_RegionCode, @p_WashCode, @p_PostCode ",
                prmBegDate, prmEndDate, prmRegion, prmWash, prmPost).ToList();

            resultlist = result;

            return resultlist;
        }

        public ActionResult CollectByPostsFilter(string region, string wash, string post, string begdate, string enddate)
        {
            List<GetCollectByPosts_Result> view = GetCollectByPosts(region, wash, post, begdate, enddate);

            return PartialView("_CollectByPostsList", view);
        }

    }
}