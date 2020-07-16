using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inspinia_MVC5.Models;

namespace Inspinia_MVC5.Controllers
{
    public class BoxesController : Controller
    {
        private ModelDb db = new ModelDb();

        List<Region> _regions = null;
        List<Wash> _washes = null;
        List<Post> _posts = null;
        List<Device> _devices = null;

        public BoxesController()
        {
            //_washes = db.Washes.ToList();
            //_regions = db.Regions.Where(r => _washes.Contains(r.Washes)).ToList();
            //_posts = db.Posts.ToList();

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

        public ActionResult BoxByWashesView()
        {
            DateTime curdate = DateTime.Now;
            ViewBag.CurDate = curdate;

            return View("BoxByWashesView");
        }

        public ActionResult _BoxByWashesList(string region, string wash, string curdate)
        {
            List<GetBoxByWashs_Result> view = GetBoxByWashes(region, wash, curdate);

            return PartialView("_BoxByWashesList", view);
        }

        public List<GetBoxByWashs_Result> GetBoxByWashes(string region, string wash, string curdate)
        {
            List<GetBoxByWashs_Result> resultlist = null;

            DateTime cdate;
            if (!DateTime.TryParse(curdate, out cdate))
                cdate = DateTime.Now;

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

            var prmCurDate = new System.Data.SqlClient.SqlParameter("@p_ReportDate", System.Data.SqlDbType.DateTime);
            prmCurDate.Value = cdate;

            var result = db.Database.SqlQuery<GetBoxByWashs_Result>
                ("GetBoxByWashs @p_ReportDate, @p_RegionCode, @p_WashCode ",
                prmCurDate, prmRegion, prmWash).ToList();

            resultlist = result;

            return resultlist;
        }

        public ActionResult BoxByWashesFilter(string region, string wash, string curdate)
        {
            List<GetBoxByWashs_Result> view = GetBoxByWashes(region, wash, curdate);

            return PartialView("_BoxByWashesList", view);
        }

        public ActionResult BoxByPostsView(string curdate, string wash)
        {
            char[] chars = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            int idx = wash.IndexOfAny(chars);
            if (idx > -1)
                wash = 'М' + wash.Substring(idx);

            Wash Wash = _washes.Find(w => w.Code == wash);

            ViewBag.Region = Wash.Region.Code;
            ViewBag.Wash = Wash.Code;

            DateTime cdate;
            if (!DateTime.TryParse(curdate, out cdate))
                cdate = DateTime.Now;

            ViewBag.CurDate = curdate;

            return View("BoxByPostsView");
        }

        public ActionResult _BoxByPostsList(string region, string wash, string post, string curdate)
        {
            List<GetBoxByPosts_Result> view = GetBoxByPosts(region, wash, post, curdate);

            return PartialView("_BoxByPostsList", view);
        }

        public List<GetBoxByPosts_Result> GetBoxByPosts(string region, string wash, string post, string curdate)
        {
            List<GetBoxByPosts_Result> resultlist = null;

            DateTime cdate;
            if (!DateTime.TryParse(curdate, out cdate))
                cdate = DateTime.Now;

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

            var prmCurDate = new System.Data.SqlClient.SqlParameter("@p_ReportDate", System.Data.SqlDbType.DateTime);
            prmCurDate.Value = cdate;

            var result = db.Database.SqlQuery<GetBoxByPosts_Result>
                ("GetBoxByPosts @p_ReportDate, @p_RegionCode, @p_WashCode, @p_PostCode ",
                prmCurDate, prmRegion, prmWash, prmPost).ToList();

            resultlist = result;

            return resultlist;
        }

        public ActionResult BoxByPostsFilter(string region, string wash, string post, string curdate)
        {
            List<GetBoxByPosts_Result> view = GetBoxByPosts(region, wash, post, curdate);

            return PartialView("_BoxByPostsList", view);
        }

    }
}