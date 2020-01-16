using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Inspinia_MVC5.Models;

namespace Inspinia_MVC5.Controllers
{
    public class MonitoringController : Controller
    {
        private ModelDb db = new ModelDb();

        List<Company> _companies = null;
        List<Post> _posts = null;

        Post post = null;

        public MonitoringController()
        {
            _posts = db.Posts.ToList();
            _companies = db.Companies.ToList();
        }

        public ActionResult MonitoringView()
        {
            return View(_companies);
        }

        public int GetSec()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://194.87.98.177/eco-api/api/dynamic/time");
            request.KeepAlive = false;
            request.ProtocolVersion = HttpVersion.Version10;
            request.Method = "GET";
            request.Accept = "application/json";

            var response = (HttpWebResponse)request.GetResponse();

            string result;
            using (StreamReader rdr = new StreamReader(response.GetResponseStream()))
            {
                result = rdr.ReadToEnd();
            }

            return int.Parse(result);
        }

        public ActionResult PostMonitoringView(int IDPost)
        {
            post = _posts.Find(x => x.IDPost == IDPost);

            int sec = GetSec();

            return PartialView("PostMonitoringView", sec);
        }
    }
}