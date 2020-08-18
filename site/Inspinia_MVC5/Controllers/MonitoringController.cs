using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Globalization;
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
        List<Device> _requiredChangers = null;

        InfoPost infopost = null;
        InfoChanger infoChanger = null;

        public MonitoringController()
        {
            _companies = db.Companies.ToList();
            _regions = new List<Region>();
            _washes = new List<Wash>();
            _posts = db.Posts.ToList();
            _devices = db.Devices.ToList();
            _requiredPosts = new List<Device>();
            _requiredChangers = new List<Device>();

            _washes = db.Washes.Where(w => w.Code == "М13" || w.Code == "М14").ToList();
            var changers = db.Changers.ToList();

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

                var chs = changers.FindAll(c => c.IDWash == w.IDWash);

                foreach (var c in chs)
                {
                    var device = _devices.Find(d => d.IDDevice == c.IDDevice);

                    if (device != null)
                    {
                        _requiredChangers.Add(device);
                    }
                }
            }

            ViewBag.Regions = _regions;
            ViewBag.Washes = _washes;
            ViewBag.Posts = _requiredPosts;
            ViewBag.Changers = _requiredChangers;
            ViewBag.Devices = _devices;
        }
        public ActionResult MonitoringChangerView()
        {
            return View(_regions);
        }

        public ActionResult _MonitoringChangerView(string codeChanger)
        {
            byte[] bytes = Encoding.GetEncoding(1251).GetBytes(codeChanger);
            var code = Encoding.GetEncoding("utf-8").GetString(bytes);

            Device changer = _devices.Find(d => d.Code == code);

            string data = $"{{\"Changer\":\"{changer.Code}\"}}";

            infoChanger = GetInfoChanger(data, changer);

            return PartialView("_MonitoringChangerView", infoChanger);
        }

        public InfoChanger GetInfoChanger(string json, Device changer)
        {
            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://194.87.98.177/postrc/api/post/heartbeat");

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://ptsv2.com/t/pev9y-1597747203/post");

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
                    //int m10 = Convert.ToInt32(response.GetResponseHeader("m10"));
                    //int b50 = Convert.ToInt32(response.GetResponseHeader("b50"));
                    //int b100 = Convert.ToInt32(response.GetResponseHeader("b100"));
                    //int b200 = Convert.ToInt32(response.GetResponseHeader("b200"));
                    //int b500 = Convert.ToInt32(response.GetResponseHeader("b500"));
                    //int b1000 = Convert.ToInt32(response.GetResponseHeader("b1000"));
                    //int b2000 = Convert.ToInt32(response.GetResponseHeader("b2000"));
                    //int box1_50 = Convert.ToInt32(response.GetResponseHeader("box1_50"));
                    //int box2_100 = Convert.ToInt32(response.GetResponseHeader("box2_100"));
                    //int box3_50 = Convert.ToInt32(response.GetResponseHeader("box3_50"));
                    //int box4_100 = Convert.ToInt32(response.GetResponseHeader("box4_100"));
                    //int badCards = Convert.ToInt32(response.GetResponseHeader("badCards"));
                    //int availableCards = Convert.ToInt32(response.GetResponseHeader("avaiableCards"));
                    //string bill = response.GetResponseHeader("bill");
                    //string coiner = response.GetResponseHeader("coiner");
                    //string bank = response.GetResponseHeader("bank");
                    //string oddMoney = response.GetResponseHeader("oddMoney");
                    //string hopper = response.GetResponseHeader("hopper");
                    //string cards = response.GetResponseHeader("cards");
                    //string issueCards = response.GetResponseHeader("issueCards");
                    //string fr = response.GetResponseHeader("fr");
                    //string printCheck = response.GetResponseHeader("printCheck");

                    //InfoChanger infoChanger = new InfoChanger(
                    //    m10,
                    //    b50,
                    //    b100,
                    //    b200,
                    //    b500,
                    //    b1000,
                    //    b2000,
                    //    box1_50,
                    //    box2_100,
                    //    box3_50,    
                    //    box4_100,
                    //    badCards,
                    //    availableCards,
                    //    bill,
                    //    coiner,
                    //    bank,
                    //    oddMoney,
                    //    hopper,
                    //    cards,
                    //    issueCards,
                    //    fr,
                    //    printCheck,
                    //    changer
                    //    );

                    //StreamReader reader = new StreamReader(response.GetResponseStream());
                    //string responseBody = reader.ReadToEnd();

                    string responseBody = "{" +
                        "\"m10\": 1," +
                        "\"b50\" : 2," +
                        "\"b100\" : 1," +
                        "\"b200\" : 3," +
                        "\"b500\" : 2," +
                        "\"b1000\" : 1," +
                        "\"b2000\" : 1," +
                        "\"box1_50\" : 4," +
                        "\"box2_100\" : 5," +
                        "\"box3_50\" : 7," +
                        "\"box4_100\" : 6," +
                        "\"badCards\" : 12," +
                        "\"availableCards\" : 42," +
                        "\"bill\" : \"активен\"," +
                        "\"coiner\" : \"не подключен\"," +
                        "\"bank\" : \"не подключен\"," +
                        "\"oddMoney\" : \"неактивен\"," +
                        "\"hopper\" : \"активен\"," +
                        "\"cards\" : \"активен\"," +
                        "\"issueCards\" : \"активен\"," +
                        "\"fr\" : \"не подключен\"," +
                        "\"printCheck\" : \"не подключен\"" +
                        "}";

                    InfoChanger infochanger = JsonConvert.DeserializeObject<InfoChanger>(responseBody);
                    infochanger.changer = changer;

                    return infochanger;
                }
                else
                {
                    return null;
                }
            }
            catch (WebException ex)
            {
                return null;
            }
        }


        #region Мониторинг за моечными постами
        public ActionResult MonitoringPostView()
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

            request.ContentType = "application/json; charset=utf-8";
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
                    StreamReader s = new StreamReader(response.GetResponseStream());

                    string res = s.ReadToEnd();

                    if(res.Length > 2)
                    {
                        res = res.Substring(1, res.Length - 2);
                    }

                    return res;
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

        public ActionResult _MonitoringPostView(string codePost)
        {
            byte[] bytes = Encoding.GetEncoding(1251).GetBytes(codePost);
            var code = Encoding.GetEncoding("utf-8").GetString(bytes);

            //post = _posts.Find(x => x.IDPost == IDPost);
            Device post = _devices.Find(d => d.Code == code);
            //device = _devices.Find(d => d.IDDevice == post.IDDevice);

            //string data = $"{{\"Post\":\"{device.Code.ToString()}\"}}";

            string data = $"{{\"Post\":\"{post.Code}\"}}";

            infopost = new InfoPost(GetBalance(data), GetFunction(data), GetState(data), post);

            return PartialView("_MonitoringPostView", infopost);
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

            return PartialView("_MonitoringPostView", infopost);
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
            return PartialView("_MonitoringPostView", infopost);
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
        #endregion
    }
}