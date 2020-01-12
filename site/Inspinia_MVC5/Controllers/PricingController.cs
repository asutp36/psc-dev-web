using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inspinia_MVC5.Models;
using Newtonsoft.Json;
using Inspinia_MVC5.Helpers;
using System.Net;
using System.Text;
using System.IO;

namespace Inspinia_MVC5.Controllers
{
    public class PricingController : Controller
    {
        private ModelDb db = new ModelDb();

        List<Region> _regions = null;

        public PricingController()
        {
            _regions = db.Regions.ToList();
        }

        public ActionResult SaveNewPrices(List<List<string>> prices, List<int> posts)
        {
            if (prices != null && posts != null)
            {
                string data = JsonConvert.SerializeObject(new ChangePricesData(posts, prices));
                string testlog = SendPrices(data);
            }
            return View("NewPricesView", _regions);
        }

        public string SendPrices(string json)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://ptsv2.com/t/97i13-1578754305/post");

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

        public ActionResult NewPricesView()
        {
            return View(_regions);
        }
    }
}