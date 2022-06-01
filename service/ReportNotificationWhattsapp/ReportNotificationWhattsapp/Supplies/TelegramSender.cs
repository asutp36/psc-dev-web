using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ReportNotificationWhattsapp.Supplies
{
    class TelegramSender
    {
        public static TelegramResponse SendMessage(string json)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://cwmon.ru/notify/api/notify/message-group");
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
                string res = JsonConvert.DeserializeObject<string>(result);
                return JsonConvert.DeserializeObject<TelegramResponse>(res);
            }
            catch (Exception e)
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string result;
                using (StreamReader rdr = new StreamReader(response.GetResponseStream()))
                {
                    result = rdr.ReadToEnd();
                }
                string res = JsonConvert.DeserializeObject<string>(result);
                return JsonConvert.DeserializeObject<TelegramResponse>(res);
            }
        }
    }
}
