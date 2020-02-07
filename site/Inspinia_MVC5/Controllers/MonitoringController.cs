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
        List<Region> _regions = null;
        List<Wash> _washes = null;
        List<Post> _posts = null;

        Post post = null;
        Region region = new Region();
        short sss = new short();
        InfoPost infopost = null;

        public MonitoringController()
        {
            _companies = db.Companies.ToList();
            _posts = db.Posts.ToList();
            _regions = db.Regions.ToList();
            _washes = db.Washes.ToList();

            ViewBag.Regions = _regions;
            ViewBag.Washes = _washes;
            ViewBag.Posts = _posts;
            ViewBag.Temp = region;
            ViewBag.X = sss;
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

        //public List<GetIncreaseOnWashes_Result> GetIncreasesWashesFromDB(string region, string wash, string post, string bdate, string edate)
        //{
        //    List<GetIncreaseOnWashes_Result> resultlist = null;

        //    DateTime begdate;
        //    if (!DateTime.TryParse(bdate, out begdate))
        //        begdate = DateTime.Today.AddDays(-100);

        //    DateTime enddate;
        //    if (!DateTime.TryParse(edate, out enddate))
        //        enddate = DateTime.Now;

        //    var prmRegion = new System.Data.SqlClient.SqlParameter("@p_RegionCode", System.Data.SqlDbType.Int);
        //    prmRegion.Value = Convert.ToInt32(region);

        //    var prmWash = new System.Data.SqlClient.SqlParameter("@p_WashCode", System.Data.SqlDbType.NVarChar);
        //    prmWash.Value = wash;

        //    var prmPost = new System.Data.SqlClient.SqlParameter("@p_PostCode", System.Data.SqlDbType.NVarChar);
        //    prmPost.Value = post;

        //    var prmBegDate = new System.Data.SqlClient.SqlParameter("@p_DateBeg", System.Data.SqlDbType.DateTime);
        //    prmBegDate.Value = bdate;

        //    var prmEndDate = new System.Data.SqlClient.SqlParameter("@p_DateEnd", System.Data.SqlDbType.DateTime);
        //    prmEndDate.Value = edate;

        //    var result = db.Database.SqlQuery<GetIncreaseOnWashes_Result>
        //        ("GetIncreaseOnWashes @p_RegionCode, @p_WashCode, @p_PostCode, @p_DateBeg, @p_DateEnd",
        //        prmRegion, prmWash, prmPost, prmBegDate, prmEndDate).ToList();

        //    return result;
        //}

        public ActionResult IncreasesWashesFilter(string region, string wash, string post, string bdate, string edate)
        {
            //List<GetIncreaseOnWashes_Result> view = GetIncreasesWashesFromDB(region, wash, post, bdate, edate);

            return PartialView("_MonitoringHistoryList");
        }

        public string GetState(string json)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://194.87.98.177/postrc/api/post/heartbeat");

            request.Timeout = 5000;

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
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return response.GetResponseHeader("HeartBeat");
                }
                else
                {
                    return "Нет доступа";
                }
            }
            catch (WebException ex)
            {
                return "Нет доступа";
            }
        }

        public string GetBalance(string json)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://194.87.98.177/postrc/api/post/getbalance");

            request.Timeout = 5000;

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
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return response.GetResponseHeader("Balance");
                }
                else
                {
                    return "Не определено";
                }
            }
            catch (WebException ex)
            {
                return "Не определено";
            }
        }

        public string GetFunction(string json)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://194.87.98.177/postrc/api/post/getfunc");

            request.Timeout = 5000;

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
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return response.GetResponseHeader("Function");
                }
                else
                {
                    return "Не определено";
                }
            }
            catch (WebException ex)
            {
                return "Не определено";
            }
        }

        public ActionResult PostMonitoringView(int IDPost)
        {
            post = _posts.Find(x => x.IDPost == IDPost);
            
            string data = $"{{\"Post\":\"{post.Code.ToString()}\"}}";

            infopost = new InfoPost(GetBalance(data), GetFunction(data), GetState(data), post);

            return PartialView("PostMonitoringView", infopost);
        }

        public ActionResult ChangeFunction(string Post, string Function, string login)
        {
            string data = JsonConvert.SerializeObject(new ChangeFunctionOnPost(Post, Function, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), login));
            string testlog = ChangeFunctionOnPost(data);

            post = _posts.Find(x => x.IDPost == Convert.ToInt32(Post));

            string req = $"{{\"Post\":\"{post.Code.ToString()}\"}}";

            infopost = new InfoPost(GetBalance(req), GetFunction(req), GetState(req), post);

            return PartialView("PostMonitoringView", infopost);
        }

        public string ChangeFunctionOnPost(string json)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://194.87.98.177/postrc/api/post/setfunc");

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
                return result + webResponse.StatusCode;
            }
        }

        public ActionResult IncreaseBalance(string Post, string sum, string login)
        {
            if (Post != null && sum != null)
            {
                string data = JsonConvert.SerializeObject(new IncreaseBalanceOnPostClass(_posts.Find(x => x.IDPost == Convert.ToInt32(Post)).Code, Convert.ToInt32(sum), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), login));
                string testlog = IncreaseBalanceOnPost(data);

                post = _posts.Find(x => x.IDPost == Convert.ToInt32(Post));

                string req = $"{{\"Post\":\"{post.Code.ToString()}\"}}";

                infopost = new InfoPost(GetBalance(req), GetFunction(req), GetState(req), post);
            }
            return PartialView("PostMonitoringView", infopost);
        }

        public string IncreaseBalanceOnPost(string json)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://194.87.98.177/postrc/api/post/incrbalance");

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