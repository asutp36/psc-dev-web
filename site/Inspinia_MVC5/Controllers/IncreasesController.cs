﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inspinia_MVC5.Models;

namespace Inspinia_MVC5.Controllers
{
    public class IncreasesController : Controller
    {
        private ModelDb db = new ModelDb();

        List<Region> _regions = null;
        List<Wash> _washes = null;
        List<Post> _posts = null;

        public IncreasesController()
        {
            _regions = db.Regions.ToList();
            _washes = db.Washes.ToList();
            _posts = db.Posts.ToList();

            ViewBag.Regions = _regions;
            ViewBag.Washes = _washes;
            ViewBag.Posts = _posts;
        }

        public ActionResult IncreasesOnPostsFilter(string region, string wash, string post, string begdate, string enddate)
        {
            List<GetIncreaseOnPosts_Result> view = GetIncreasesOnPosts(region, wash, post, begdate, enddate);

            return PartialView("_IncreasesOnPostsList", view);
        }

        public ActionResult IncreasesOnWashesFilter(string region, string wash, string begdate, string enddate)
        {
            List<GetIncreaseOnWashes_Result> view = GetIncreasesOnWashes(region, wash, begdate, enddate);

            return PartialView("_IncreasesOnWashesList", view);
        }

        public List<GetIncreaseOnPosts_Result> GetIncreasesOnPosts(string region, string wash, string post, string begdate, string enddate)
        {
            List<GetIncreaseOnPosts_Result> resultlist = null;

            DateTime bdate;
            if (!DateTime.TryParse(begdate, out bdate))
                bdate = DateTime.Today.AddYears(-10);

            DateTime edate;
            if (!DateTime.TryParse(enddate, out edate))
                edate = DateTime.Now;

            var prmRegion = new System.Data.SqlClient.SqlParameter("@p_RegionCode", System.Data.SqlDbType.Int);
            prmRegion.Value = Convert.ToInt32(region);

            var prmWash = new System.Data.SqlClient.SqlParameter("@p_WashCode", System.Data.SqlDbType.NVarChar);
            prmWash.Value = wash;

            var prmPost = new System.Data.SqlClient.SqlParameter("@p_PostCode", System.Data.SqlDbType.NVarChar);
            prmPost.Value = post;

            var prmBegDate = new System.Data.SqlClient.SqlParameter("@p_DateBeg", System.Data.SqlDbType.DateTime);
            prmBegDate.Value = bdate;

            var prmEndDate = new System.Data.SqlClient.SqlParameter("@p_DateEnd", System.Data.SqlDbType.DateTime);
            prmEndDate.Value = edate;

            var result = db.Database.SqlQuery<GetIncreaseOnPosts_Result>
                ("GetIncreaseOnPosts @p_RegionCode, @p_WashCode, @p_PostCode, @p_DateBeg, @p_DateEnd",
                prmRegion, prmWash, prmPost, prmBegDate, prmEndDate).ToList();

            resultlist = result;

            return resultlist;
        }

        public List<GetIncreaseOnWashes_Result> GetIncreasesOnWashes(string region, string wash, string begdate, string enddate)
        {
            List<GetIncreaseOnWashes_Result> resultlist = null;

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
            prmPost.Value = 0;

            var prmBegDate = new System.Data.SqlClient.SqlParameter("@p_DateBeg", System.Data.SqlDbType.DateTime);
            prmBegDate.Value = bdate;

            var prmEndDate = new System.Data.SqlClient.SqlParameter("@p_DateEnd", System.Data.SqlDbType.DateTime);
            prmEndDate.Value = edate;

            var result = db.Database.SqlQuery<GetIncreaseOnWashes_Result>
                ("GetIncreaseOnWashes @p_RegionCode, @p_WashCode, @p_PostCode, @p_DateBeg, @p_DateEnd",
                prmRegion, prmWash, prmPost, prmBegDate, prmEndDate).ToList();

            resultlist = result;

            return resultlist;
        }

        public ActionResult IncreasesOnPostsView(string begdate, string enddate, string wash)
        {
            Wash Wash = _washes.Find(w => w.IDWash == Convert.ToInt32(wash));

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

            return View("IncreasesOnPostsView");
        }

        public ActionResult _IncreasesOnPostsList(string region, string wash, string post, string begdate, string enddate)
        {
            List<GetIncreaseOnPosts_Result> view = GetIncreasesOnPosts(region, wash, post, begdate, enddate);

            return PartialView("_IncreasesOnPostsList", view);
        }

        public ActionResult _IncreasesLipetskList(string date)
        {
            List<GetDayIncrease_Result> view = GetIncreasesLipetsk(date);

            return PartialView("_IncreasesLipetskList", view);
        }

        public ActionResult IncreasesLipetskFilter(string date)
        {
            List<GetDayIncrease_Result> view = GetIncreasesLipetsk(date);

            return PartialView("_IncreasesLipetskList", view);
        }

        public List<GetDayIncrease_Result> GetIncreasesLipetsk(string date)
        {
            List<GetDayIncrease_Result> resultlist = null;

            DateTime bdate;
            if (!DateTime.TryParse(date, out bdate))
                bdate = DateTime.Now;

            DateTime edate = bdate.AddDays(1);

            edate = edate.AddSeconds(-1);

            var prmBegDate = new System.Data.SqlClient.SqlParameter("@p_DateBeg", System.Data.SqlDbType.DateTime);
            prmBegDate.Value = bdate;

            var prmEndDate = new System.Data.SqlClient.SqlParameter("@p_DateEnd", System.Data.SqlDbType.DateTime);
            prmEndDate.Value = edate;

            var result = db.Database.SqlQuery<GetDayIncrease_Result>
                ("GetDayIncrease @p_DateBeg, @p_DateEnd",
                prmBegDate, prmEndDate).ToList();

            resultlist = result;

            return resultlist;
        }

        public ActionResult IncreasesLipetskView()
        {
            return View("IncreasesLipetskView");
        }

        public ActionResult IncreasesOnWashesView()
        {
            DateTime edate = DateTime.Now;
            ViewBag.EndDate = edate;

            return View("IncreasesOnWashesView");
        }

        public ActionResult _IncreasesOnWashesList(string region, string wash, string begdate, string enddate)
        {
            List<GetIncreaseOnWashes_Result> view = GetIncreasesOnWashes(region, wash, begdate, enddate);

            return PartialView("_IncreasesOnWashesList", view);
        }
    }
}