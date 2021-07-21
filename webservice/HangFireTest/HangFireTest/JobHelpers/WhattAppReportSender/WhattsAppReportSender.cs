using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HangFireTest.JobHelpers.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace HangFireTest.JobHelpers.WhattAppReportSender
{
    public class WhattsAppReportSender
    {
        public static void CreateReportJob(int recipient)
        {
            using (HangfireDbContext context = new HangfireDbContext())
            {
                var waRecipientData = context.WhattsAppRecipients.Where(r => r.WaRecipients == recipient).FirstOrDefault();
                string msg = "";

                using (WashCompanyDbContext wcContext = new WashCompanyDbContext())
                {
                    SqlParameter wash = new SqlParameter("@wash", System.Data.SqlDbType.NVarChar);
                    wash.Value = waRecipientData.WashCode;
                    SqlParameter date = new SqlParameter("@date", System.Data.SqlDbType.DateTime);
                    date.Value = DateTime.Now.AddDays(-1).Date.ToString("yyyy-MM-dd HH:mm:ss");

                    var sp_result = wcContext.Set<GetDayReportIncrease_Result>().FromSqlRaw("GetDayReportIncrease @wash, @date", wash, date).ToList();
                    msg = sp_result[0].msg.Replace(" П", "\nП").Replace(": ", ":\n").Insert(sp_result[0].msg.IndexOf("за ") + 14, "\n");
                };

                String[] chats = waRecipientData.ChatId.Split(new Char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string chat in chats)
                {
                    MessageToSend message = new MessageToSend();
                    message.chatId = chat;
                    message.body = msg;

                    bool sendResult = SendReport(JsonConvert.SerializeObject(message));
                }
            };



            string data = $"this is bacground job at {DateTime.Now} with recipient = {recipient}!";

            //Console.WriteLine("result is " + SendReport(data).ToString());
        }

        private static bool SendReport(string json)
        {
            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://eu33.chat-api.com/instance27633/sendMessage?token=0qgid5wjmhb8vw7d");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://ptsv2.com/t/2hlhv-1626801511/post");
            request.KeepAlive = false;
            request.ProtocolVersion = HttpVersion.Version10;
            request.Method = "POST";

            byte[] postBytes = Encoding.UTF8.GetBytes(json);

            request.ContentType = "application/json; charset=UTF-8";
            request.Accept = "application/json";
            request.ContentLength = postBytes.Length;

            Stream requestStream = request.GetRequestStream();

            requestStream.Write(postBytes, 0, postBytes.Length);
            requestStream.Close();

            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string result;
                using (StreamReader rdr = new StreamReader(response.GetResponseStream()))
                {
                    result = rdr.ReadToEnd();
                }
                //return JsonConvert.DeserializeObject<ResponseSendMessage>(result);
                return true;
            }
            catch (Exception e)
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string result;
                using (StreamReader rdr = new StreamReader(response.GetResponseStream()))
                {
                    result = rdr.ReadToEnd();
                }
                //return JsonConvert.DeserializeObject<ResponseSendMessage>(result);
                return false;
            }
        }
    }
}
