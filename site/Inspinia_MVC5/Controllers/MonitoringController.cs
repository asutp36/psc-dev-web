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
        List<Device> _devices = null;
        List<Device> _requiredPosts = null;

        InfoPost infopost = null;

        public MonitoringController()
        {
            _companies = db.Companies.ToList();
            _regions = new List<Region>();
            _washes = new List<Wash>();
            _posts = db.Posts.ToList();
            _devices = db.Devices.ToList();
            _requiredPosts = new List<Device>();

            _washes = db.Washes.Where(w => w.Code == "М13" || w.Code == "М14").ToList();

            foreach (Wash w in _washes)
            {
                if (!_regions.Contains(w.Region))
                    _regions.Add(w.Region);

                foreach (var p in w.Posts)
                {
                    Device device = _devices.Find(d => d.IDDevice == p.IDDevice && d.IDDeviceType == 2);

                    if (device != null)
                    {
                        _requiredPosts.Add(device);
                    }
                }
            }

            //foreach (var c in _companies)
            //{
            //    foreach(var r in c.Regions)
            //    {
            //        foreach(var w in r.Washes)
            //        {
            //            for (int i = w.Posts.Count - 1; i >= 0; i--)
            //            {
            //                if (w.Posts.ElementAt(i).IDDevice != null)
            //                {
            //                    if (_devices.Find(d => d.IDDevice == w.Posts.ElementAt(i).IDDevice).IDDeviceType != 2)
            //                    {
            //                        w.Posts.Remove(w.Posts.ElementAt(i));
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}

            ViewBag.Regions = _regions;
            ViewBag.Washes = _washes;
            ViewBag.Posts = _requiredPosts;
            ViewBag.Devices = _devices;
        }

        public ActionResult MonitoringHistoryWashesView()
        {
            return View();
        }

        public ActionResult _MonitoringHistoryList()
        {
            return PartialView("_MonitoringHistoryWashesList");
        }

        public ActionResult MonitoringView()
        {
            return View(_regions);
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
                    string s = response.GetResponseHeader("Function");

                    byte[] bytes = Encoding.GetEncoding(1252).GetBytes(s);
                    s = Encoding.GetEncoding("utf-8").GetString(bytes);

                    return s;

                    //return response.GetResponseHeader("Function");
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

        public ActionResult PostMonitoringView(string codePost)
        {
            //post = _posts.Find(x => x.IDPost == IDPost);
            Device post = _devices.Find(d => d.Code == codePost);
            //device = _devices.Find(d => d.IDDevice == post.IDDevice);

            //string data = $"{{\"Post\":\"{device.Code.ToString()}\"}}";

            string data = $"{{\"Post\":\"{post.Code}\"}}";

            infopost = new InfoPost(GetBalance(data), GetFunction(data), GetState(data), post);

            return PartialView("PostMonitoringView", infopost);
        }

        public ActionResult ChangeFunction(string Post, string Function, string login)
        {
            string data = JsonConvert.SerializeObject(new ChangeFunctionOnPost(Post, Function, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), login));
            string testlog = ChangeFunctionOnPost(data);

            Device post = _devices.Find(d => d.Code == Post);

            //post = _posts.Find(x => x.IDPost == Convert.ToInt32(Post));
            //device = _devices.Find(d => d.IDDevice == post.IDDevice);

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
                //Post p = _posts.Find(x => x.IDPost == Convert.ToInt32(Post));
                //device = _devices.Find(d => d.IDDevice == p.IDDevice);

                Device post = _devices.Find(d => d.Code == Post);

                string data = JsonConvert.SerializeObject(new IncreaseBalanceOnPostClass(post.Code, Convert.ToInt32(sum), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), login));
                string testlog = IncreaseBalanceOnPost(data);

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