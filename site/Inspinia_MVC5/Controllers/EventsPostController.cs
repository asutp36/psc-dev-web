//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//using Inspinia_MVC5.Models;

//namespace Inspinia_MVC5.Controllers
//{
//    public class EventsPostController : Controller
//    {
//        private ModelDb db = new ModelDb();

//        List<Region> _regions = null;
//        List<Wash> _washes = null;
//        List<Post> _posts = null;
//        List<EventKind> _eventKinds = null;

//        public EventsPostController()
//        {
//            _washes = db.Washes.Where(w => w.Code == "М13" || w.Code == "М14").ToList();

//            _regions = new List<Region>();
//            _posts = new List<Post>();

//            _eventKinds = db.EventKinds.Where(e => e.Code == "cashincrease" || e.Code == "mode" || e.Code == "collect").ToList();

//            foreach (Wash w in _washes)
//            {
//                if (!_regions.Contains(w.Region))
//                    _regions.Add(w.Region);

//                _posts.AddRange(w.Posts);
//            }

//            ViewBag.Regions = _regions;
//            ViewBag.Washes = _washes;
//            ViewBag.Posts = _posts;
//            ViewBag.Events = _eventKinds;
//        }

//        public ActionResult EventsByPostsView(string begdate, string enddate, string post)
//        {
//            //Post Post = _posts.Find(w => w.Code == post);

//            //ViewBag.Region = Post.Wash.Region.Code;
//            //ViewBag.Wash = Post.Wash.Code;
//            //ViewBag.Post = Post.Code;

//            DateTime bdate;
//            if (!DateTime.TryParse(begdate, out bdate))
//                bdate = DateTime.Today.AddYears(-10);

//            DateTime edate;
//            if (!DateTime.TryParse(enddate, out edate))
//                edate = DateTime.Now;

//            ViewBag.BegDate = begdate;
//            ViewBag.EndDate = enddate;

//            return View("EventsByPostsView");
//        }
//        public ActionResult _EventsByPostsList(string post, string begdate, string enddate, string operation)
//        {
//            List<GetEventsByPost_Result> view = GetEventsByPosts(post, begdate, enddate, operation);

//            return PartialView("_EventsByPostsList", view);
//        }

//        public List<GetEventsByPost_Result> GetEventsByPosts(string post, string begdate, string enddate, string operation)
//        {
//            List<GetEventsByPost_Result> resultlist = null;

//            DateTime bdate;
//            if (!DateTime.TryParse(begdate, out bdate))
//                bdate = DateTime.Today.AddYears(-10);

//            DateTime edate;
//            if (!DateTime.TryParse(enddate, out edate))
//                edate = DateTime.Now;

//            var prmPost = new System.Data.SqlClient.SqlParameter("@p_PostCode", System.Data.SqlDbType.NVarChar);
//            if (post == null || post == "")
//            {
//                post = "nenene";
//            }
//            prmPost.Value = post;

//            var prmBegDate = new System.Data.SqlClient.SqlParameter("@p_DateBeg", System.Data.SqlDbType.DateTime);
//            prmBegDate.Value = bdate;

//            var prmEndDate = new System.Data.SqlClient.SqlParameter("@p_DateEnd", System.Data.SqlDbType.DateTime);
//            prmEndDate.Value = edate;

//            var prmOperation = new System.Data.SqlClient.SqlParameter("@p_KindEventCode", System.Data.SqlDbType.NVarChar);
//            if (operation == null)
//            {
//                operation = "";
//            }
//            prmOperation.Value = operation;

//            var result = db.Database.SqlQuery<GetEventsByPost_Result>
//                ("GetEventsByPost @p_DateBeg, @p_DateEnd, @p_PostCode, @p_KindEventCode ",
//                prmBegDate, prmEndDate, prmPost, prmOperation).ToList();

//            resultlist = result;

//            return resultlist;
//        }

//        public ActionResult EventsByPostsFilter(string post, string begdate, string enddate, string operation)
//        {
//            List<GetEventsByPost_Result> view = GetEventsByPosts(post, begdate, enddate, operation);

//            return PartialView("_EventsByPostsList", view);
//        }
//    }
//}