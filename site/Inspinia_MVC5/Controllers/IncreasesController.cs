﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Web.Management;
using System.Web.Mvc;
using Inspinia_MVC5.Models;
using Microsoft.SqlServer.Server;

namespace Inspinia_MVC5.Controllers
{
    public class IncreasesController : Controller
    {
        private ModelDb db = new ModelDb();

        List<Region> _regions = null;
        List<Wash> _washes = null;
        List<Post> _posts = null;
        List<Device> _devices = null;
        List<Device> _requiredPosts = null;

        public IncreasesController()
        {
            //_washes = db.Washes.ToList();
            //_regions = db.Regions.Where(r => _washes.Contains(r.Washes)).ToList();
            //_posts = db.Posts.ToList();

            _washes = db.Washes.Where(w => w.Code == "М13" || w.Code == "М14").ToList();
            _devices = db.Devices.ToList();

            _regions = new List<Region>();
            _posts = db.Posts.ToList();
            _requiredPosts = new List<Device>();

            foreach (Wash w in _washes)
            {
                if (!_regions.Contains(w.Region))
                    _regions.Add(w.Region);

                //удаляет пылесосы из списка постов
                //for (int i = w.Posts.Count - 1; i >= 0; i--)
                //{
                //    if (_devices.Find(d => d.IDDevice == w.Posts.ElementAt(i).IDDevice).IDDeviceType != 2)
                //    {
                //        w.Posts.Remove(w.Posts.ElementAt(i));
                //    }
                //}

                foreach(var p in w.Posts)
                {
                    Device device = _devices.Find(d => d.IDDevice == p.IDDevice);

                    if(device != null)
                    {
                        _requiredPosts.Add(device);
                    }
                }
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
            ViewBag.Posts = _requiredPosts;
            ViewBag.Devices = _devices;
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
                bdate = new DateTime(2019, 1, 1);

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
                bdate = new DateTime(2019, 1, 1);

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
                bdate = new DateTime(2019, 1, 1);

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
                bdate = DateTime.Today;

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

        public ActionResult IncreasesByWashesView()
        {
            DateTime edate = DateTime.Now;
            ViewBag.EndDate = edate;

            return View("IncreasesByWashesView");
        }

        public ActionResult _IncreasesByWashesList(string region, string wash, string begdate, string enddate)
        {
            List<GetIncreaseByWashs_Result> view = GetIncreasesByWashes(region, wash, begdate, enddate);

            return PartialView("_IncreasesByWashesList", view);
        }

        public List<GetIncreaseByWashs_Result> GetIncreasesByWashes(string region, string wash, string begdate, string enddate)
        {
            List<GetIncreaseByWashs_Result> resultlist = null;

            DateTime bdate;
            if (!DateTime.TryParse(begdate, out bdate))
                bdate = new DateTime(2019, 1, 1);

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

            var result = db.Database.SqlQuery<GetIncreaseByWashs_Result>
                ("GetIncreaseByWashs @p_DateBeg, @p_DateEnd, @p_RegionCode, @p_WashCode ",
                prmBegDate, prmEndDate, prmRegion, prmWash).ToList();

            resultlist = result;

            return resultlist;
        }

        public ActionResult IncreasesByWashesFilter(string region, string wash, string begdate, string enddate)
        {
            List<GetIncreaseByWashs_Result> view = GetIncreasesByWashes(region, wash, begdate, enddate);

            return PartialView("_IncreasesByWashesList", view);
        }

        public ActionResult IncreasesByPostsView(string begdate, string enddate, string wash)
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
                bdate = new DateTime(2019, 1, 1);

            DateTime edate;
            if (!DateTime.TryParse(enddate, out edate))
                edate = DateTime.Now;

            ViewBag.BegDate = begdate;
            ViewBag.EndDate = enddate;

            return View("IncreasesByPostsView");
        }

        public ActionResult _IncreasesByPostsList(string region, string wash, string post, string begdate, string enddate)
        {
            List<GetIncreaseByPosts_Result> view = GetIncreasesByPosts(region, wash, post, begdate, enddate);

            return PartialView("_IncreasesByPostsList", view);
        }

        public List<GetIncreaseByPosts_Result> GetIncreasesByPosts(string region, string wash, string post, string begdate, string enddate)
        {
            List<GetIncreaseByPosts_Result> resultlist = null;

            DateTime bdate;
            if (!DateTime.TryParse(begdate, out bdate))
                bdate = new DateTime(2019, 1, 1);

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

            var result = db.Database.SqlQuery<GetIncreaseByPosts_Result>
                ("GetIncreaseByPosts @p_DateBeg, @p_DateEnd, @p_RegionCode, @p_WashCode, @p_PostCode ",
                prmBegDate, prmEndDate, prmRegion, prmWash, prmPost).ToList();

            resultlist = result;

            return resultlist;
        }

        public ActionResult IncreasesByPostsFilter(string region, string wash, string post, string begdate, string enddate)
        {
            List<GetIncreaseByPosts_Result> view = GetIncreasesByPosts(region, wash, post, begdate, enddate);

            return PartialView("_IncreasesByPostsList", view);
        }

        public ActionResult IncreasesByEventsView(string begdate, string enddate, string post)
        {
            //Post Post = _posts.Find(w => w.Code == post);

            //ViewBag.Region = Post.Wash.Region.Code;
            //ViewBag.Wash = Post.Wash.Code;
            //ViewBag.Post = Post.Code;

            Device device = _devices.Find(d => d.Code == post);

            var wash = _posts.Find(p => p.IDDevice == device.IDDevice).Wash;

            if (wash != null)
            {
                ViewBag.Region = wash.Region.Code;
                ViewBag.Wash = wash.Code;
            }
            else
            {
                ViewBag.Region = "";
                ViewBag.Wash = "";
            }

            ViewBag.Post = device.Code;

            DateTime bdate;
            if (!DateTime.TryParse(begdate, out bdate))
                bdate = new DateTime(2019, 1, 1);

            DateTime edate;
            if (!DateTime.TryParse(enddate, out edate))
                edate = DateTime.Now;

            ViewBag.BegDate = begdate;
            ViewBag.EndDate = enddate;

            return View("IncreasesByEventsView");
        }

        public ActionResult _IncreasesByEventsList(string region, string wash, string post, string begdate, string enddate)
        {
            List<GetIncreaseByEvents_Result> view = GetIncreasesByEvents(region, wash, post, begdate, enddate);

            return PartialView("_IncreasesByEventsList", view);
        }

        public List<GetIncreaseByEvents_Result> GetIncreasesByEvents(string region, string wash, string post, string begdate, string enddate)
        {
            List<GetIncreaseByEvents_Result> resultlist = null;

            DateTime bdate;
            if (!DateTime.TryParse(begdate, out bdate))
                bdate = new DateTime(2019, 1, 1);

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

            var result = db.Database.SqlQuery<GetIncreaseByEvents_Result>
                ("GetIncreaseByEvents @p_DateBeg, @p_DateEnd, @p_RegionCode, @p_WashCode, @p_PostCode ",
                prmBegDate, prmEndDate, prmRegion, prmWash, prmPost).ToList();

            resultlist = result;

            return resultlist;
        }

        public ActionResult IncreasesByEventsFilter(string region, string wash, string post, string begdate, string enddate)
        {
            List<GetIncreaseByEvents_Result> view = GetIncreasesByEvents(region, wash, post, begdate, enddate);

            return PartialView("_IncreasesByEventsList", view);
        }

        public ActionResult _IncreasesSum(string begdate, string enddate)
        {
            int sum = 0;

            List<GetIncreaseByWashs_Result> res = GetIncreasesByWashes("", "", begdate, enddate);

            foreach(var r in res)
            {
                sum += r.sum;
            }

            var nf = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nf.NumberGroupSeparator = " ";

            string result = sum.ToString("#,0", nf);

            return PartialView("_IncreasesSum", result);
        }
    }
}