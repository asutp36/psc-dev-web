using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Inspinia_MVC5.Helpers;
using Inspinia_MVC5.Models;
using Newtonsoft.Json;

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

        public ActionResult MonitoringHistoryView()
        {
            return View();
        }

        public ActionResult _MonitoringHistoryList()
        {
            return PartialView("_MonitoringHistoryList");
        }

        public ActionResult MonitoringView()
        {
            return View(_companies);
        }

        public void GetIncreasesHistoryFromDB(string login, string sum, string region, string wash, string post, string bdate, string edate)
        {

        }

        public ActionResult IncreasesPostsFilter(string login, string sum, string region, string wash, string post, string bdate, string edate)
        {
            //вызов хранимой процедуры для фильтрации пополнений

            return PartialView("_MonitoringHistoryList");
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
            post.Code = GetSec().ToString();

            return PartialView("PostMonitoringView", post);
        }

        public ActionResult IncreaseBalance(string Post, string sum, string login)
        {
            if (Post != null && sum != null)
            {
                string data = JsonConvert.SerializeObject(new IncreaseBalanceOnPostClass(_posts.Find(x => x.IDPost == Convert.ToInt32(Post)).Code, Convert.ToInt32(sum), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), login));
                string testlog = IncreaseBalanceOnPost(data);

                post = _posts.Find(x => x.IDPost == Convert.ToInt32(Post));
                post.Code = GetSec().ToString();
            }
            return View("PostMonitoringView", post);
        }

        public string IncreaseBalanceOnPost(string json)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://194.87.98.177/postrc/api/post/incrbalance");

            //request.Host = "api.myeco24.ru";
            request.KeepAlive = false;
            request.ProtocolVersion = HttpVersion.Version10;
            request.Method = "POST";

            byte[] postBytes = Encoding.UTF8.GetBytes(json);

            request.ContentType = "application/json";
            request.Accept = "application/json";
            request.ContentLength = postBytes.Length;

            Stream requestStream = request.GetRequestStream();

            requestStream.Write(postBytes, 0, postBytes.Length);
            requestStream.Close();

            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return response.ToString();
                }
                else
                {
                    string result;
                    using (StreamReader rdr = new StreamReader(response.GetResponseStream()))
                    {
                        result = rdr.ReadToEnd();
                    }

                    return String.Format("httpStatusCode: {0}; {1}", response.StatusCode, result);
                }
            }
            catch (WebException ex)
            {
                HttpWebResponse webResponse = (HttpWebResponse)ex.Response;

                string result;
                using (StreamReader rdr = new StreamReader(webResponse.GetResponseStream()))
                {
                    result = rdr.ReadToEnd();
                }
                return result + "\nStatusCode: " + webResponse.StatusCode;
            }
        }
    }
}