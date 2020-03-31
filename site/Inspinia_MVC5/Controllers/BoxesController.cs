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

        public BoxesController()
        {
            //_washes = db.Washes.ToList();
            //_regions = db.Regions.Where(r => _washes.Contains(r.Washes)).ToList();
            //_posts = db.Posts.ToList();

            _washes = db.Washes.Where(w => w.Code == "М13" || w.Code == "М14").ToList();

            _regions = new List<Region>();
            _posts = new List<Post>();
            foreach (Wash w in _washes)
            {
                if (!_regions.Contains(w.Region))
                    _regions.Add(w.Region);

                _posts.AddRange(w.Posts);
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
    }
}