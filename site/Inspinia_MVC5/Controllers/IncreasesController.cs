using System;
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
                ("GetIncreaseOnWashes @p_RegionCode, @p_WashCode, @p_PostCode, @p_DateBeg, @p_DateEnd",
                prmRegion, prmWash, prmPost, prmBegDate, prmEndDate).ToList();

            resultlist = result;

            return resultlist;
        }

        public ActionResult _IncereasesOnPostsList(string region, string wash, string post, string begdate, string enddate)
        {
            List<GetIncreaseOnPosts_Result> view = GetIncreasesOnPosts(region, wash, post, begdate, enddate)

            return PartialView("_IncreasesOnPostsList", view);
        }

        public ActionResult IncreasesOnPostsView()
        {
            return View("IncreasesOnPostsView");
        }

        // GET: Increases
        public ActionResult Index()
        {
            return View();
        }
    }
}