using Inspinia_MVC5.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Inspinia_MVC5.Controllers
{
    public class NotificationsController : Controller
    {
        public ActionResult NotificationsToReceiversSend(string receiver, string theme, string text)
        {
            if (receiver != null && text != null)
            {
                string data = JsonConvert.SerializeObject(new NotificationClass(receiver, theme, text));
                string testlog = SendRequest(data);
            }
            return PartialView("_NotificationsFormView");
        }

        public ActionResult NotificationsToAllSend(string theme, string text)
        {
            if (text != null)
            {
                string data = JsonConvert.SerializeObject(new NotificationClass("FORALL", theme, text));
                string testlog = SendRequest(data);
            }
            return PartialView("_NotificationsFormView");
        }

        public string SendRequest(string json)
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

        public void GetNotificationsHIstoryFromDB(string phone, string text, string bdate, string edate)
        {

        }

        public ActionResult NotificationsFilter(string phone, string text, string bdate, string edate)
        {
            //вызов хранимой процедуры для фильтрации оповещений

            return PartialView("_NotificationsHistoryList");
        }
        
        public ActionResult _NotificationsFormView()
        {
            return PartialView("_NotificationsFormView");
        }

        public ActionResult NotificationsSendingView()
        {
            return View();
        }

        public ActionResult _NotificationsHistoryList()
        {
            return PartialView("_NotificationsHistoryList");
        }

        public ActionResult NotificationsHistoryView()
        {
            return View();
        }
    }
}