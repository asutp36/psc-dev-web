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
using Inspinia_MVC5.Models;

namespace Inspinia_MVC5.Controllers
{
    public class NotificationsController : Controller
    {
        private ModelDb db = new ModelDb();

        List<Owner> _owners = null;
        List<NoticeStatu> _noticeStatus = null;
        List<NoticeRecipientsGroup> _noticeRecipients = null;

        public NotificationsController()
        {
            _noticeStatus = db.NoticeStatus.ToList();
            _owners = db.Owners.ToList();
            _noticeRecipients = db.NoticeRecipientsGroups.ToList();

            ViewBag.Recepients = _noticeRecipients;
            ViewBag.Notices = _noticeStatus;
            ViewBag.Owners = _owners;
        }

        public ActionResult NotificationsToReceiversSend(string sender, bool isPhone, string receiver, string theme, string text)
        {
            if (receiver != null && text != null)
            {
                string data = JsonConvert.SerializeObject(new NotificationClass(sender, isPhone, receiver, theme, text));
                string testlog = SendRequest(data);
            }
            return PartialView("_NotificationsFormView");
        }

        public ActionResult NotificationsToAllSend(string sender, bool isPhone, string theme, string text)
        {
            if (text != null)
            {
                string data = JsonConvert.SerializeObject(new NotificationClass(sender, isPhone, "FORALL", theme, text));
                string testlog = SendRequest(data);
            }
            return PartialView("_NotificationsFormView");
        }

        public string SendRequest(string json)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://194.87.98.177/notify/api/notify/message");
            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://ptsv2.com/t/jsleg-1580653259/post");

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

        public List<GetNoticeHistoryList_Result> GetNotificationsHistoryFromDB(
            string sender, string recbegdate, string recenddate, string reccode, string recname, string sbegdate, 
            string senddate, string noticestatuscode, string noticestatusname, string message)
        {
            List<GetNoticeHistoryList_Result> resultlist = null;

            DateTime receiverbegdate;
            if (!DateTime.TryParse(recbegdate, out receiverbegdate))
                receiverbegdate = DateTime.Today.AddDays(-100);

            DateTime receiverenddate;
            if (!DateTime.TryParse(recenddate, out receiverenddate))
                receiverenddate = DateTime.Now;

            DateTime sentbegdate;
            if (!DateTime.TryParse(sbegdate, out sentbegdate))
                sentbegdate = DateTime.Today.AddDays(-100);

            DateTime sentenddate;
            if (!DateTime.TryParse(senddate, out sentenddate))
                sentenddate = DateTime.Now;

            var prmSender = new System.Data.SqlClient.SqlParameter("@p_Sender", System.Data.SqlDbType.NVarChar);
            prmSender.Value = sender;

            var prmRecBegDate = new System.Data.SqlClient.SqlParameter("@p_DateReceiveBeg", System.Data.SqlDbType.DateTime);
            prmRecBegDate.Value = receiverbegdate;

            var prmRecEndDate = new System.Data.SqlClient.SqlParameter("@p_DateReceiveEnd", System.Data.SqlDbType.DateTime);
            prmRecEndDate.Value = receiverenddate;

            var prmRecCode = new System.Data.SqlClient.SqlParameter("@p_RecipientsCode", System.Data.SqlDbType.NVarChar);
            prmRecCode.Value = reccode;

            var prmRecName = new System.Data.SqlClient.SqlParameter("@p_RecipientsName", System.Data.SqlDbType.NVarChar);
            prmRecName.Value = recname;

            var prmSentBegDate = new System.Data.SqlClient.SqlParameter("@p_DateSentBeg", System.Data.SqlDbType.DateTime);
            prmSentBegDate.Value = sentbegdate;

            var prmSentEndDate = new System.Data.SqlClient.SqlParameter("@p_DateSentEnd", System.Data.SqlDbType.DateTime);
            prmSentEndDate.Value = sentenddate;

            var prmNoticeStatusCode = new System.Data.SqlClient.SqlParameter("@p_NoticeStatusCode", System.Data.SqlDbType.NVarChar);
            prmNoticeStatusCode.Value = noticestatuscode;

            var prmNoticeStatusName = new System.Data.SqlClient.SqlParameter("@p_NoticeStatusName", System.Data.SqlDbType.NVarChar);
            prmNoticeStatusName.Value = noticestatusname;

            var prmMessage = new System.Data.SqlClient.SqlParameter("@p_Message", System.Data.SqlDbType.NVarChar);
            prmMessage.Value = message;

            var result = db.Database.SqlQuery<GetNoticeHistoryList_Result>
                ("GetNoticeHistoryList @p_Sender, @p_DateReceiveBeg, @p_DateReceiveEnd, @p_RecipientsCode, " +
                "@p_RecipientsName, @p_DateSentBeg, @p_DateSentEnd, @p_NoticeStatusCode, @p_NoticeStatusName, @p_Message",
                prmSender, prmRecBegDate, prmRecEndDate, prmRecCode, prmRecName, prmSentBegDate, prmSentEndDate,
                prmNoticeStatusCode, prmNoticeStatusName, prmMessage)
                .ToList();

            resultlist = result;

            return resultlist;

        }

        public ActionResult NotificationsFilter(
            string sender, string recbegdate, string recenddate, string reccode, string recname, string sbegdate,
            string senddate, string noticestatuscode, string noticestatusname, string message)
        {
            List<GetNoticeHistoryList_Result> view = GetNotificationsHistoryFromDB(
                sender, recbegdate, recenddate, reccode, recname, sbegdate,
                senddate, noticestatuscode, noticestatusname, message);
            

            return PartialView("_NotificationsHistoryList", view);
        }
        
        public ActionResult _NotificationsFormView()
        {
            return PartialView("_NotificationsFormView");
        }

        public ActionResult NotificationsSendingView()
        {
            return View();
        }

        public ActionResult _NotificationsHistoryList(
            string sender, string recbegdate, string recenddate, string reccode, string recname, string sbegdate,
            string senddate, string noticestatuscode, string noticestatusname, string message)
        {
            List<GetNoticeHistoryList_Result> view = GetNotificationsHistoryFromDB(
                sender, recbegdate, recenddate, reccode, recname, sbegdate,
                senddate, noticestatuscode, noticestatusname, message);

            return PartialView("_NotificationsHistoryList", view);
        }

        public ActionResult NotificationsHistoryView()
        {
            return View();
        }
    }
}