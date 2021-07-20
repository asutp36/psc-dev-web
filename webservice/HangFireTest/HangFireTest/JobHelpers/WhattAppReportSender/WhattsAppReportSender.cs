using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HangFireTest.JobHelpers.WhattAppReportSender
{
    public class WhattsAppReportSender
    {
        public static void CreateReportJob(int recipient)
        {
            string data = $"this is bacground job at {DateTime.Now} with recipient = {recipient}!";

            Console.WriteLine("result is " + SendReport(data).ToString());
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
