using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace PostControllingService.Controllers.Supplies
{
    public class HttpSender
    {
        public static HttpSenderResponse SendPost(string url, string json)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
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
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return new HttpSenderResponse(response.StatusCode, response.ToString());
                }
                else
                {
                    string result;
                    using (StreamReader rdr = new StreamReader(response.GetResponseStream()))
                    {
                        result = rdr.ReadToEnd();
                    }

                    return new HttpSenderResponse(response.StatusCode, result);
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
                return new HttpSenderResponse(webResponse.StatusCode, result);
            }
        }

        public static int GetInt(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.KeepAlive = false;
            request.ProtocolVersion = HttpVersion.Version10;
            request.Method = "GET";

            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string result;
                using (StreamReader rdr = new StreamReader(response.GetResponseStream()))
                {
                    result = rdr.ReadToEnd();
                }

                return int.Parse(result);
            }
            catch(WebException ex)
            {
                HttpWebResponse webResponse = (HttpWebResponse)ex.Response;
                
                return -1;
            }
        }

        public static HttpSenderResponse SendGet(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.KeepAlive = false;
            request.ProtocolVersion = HttpVersion.Version10;
            request.Method = "GET";

            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string result;
                using (StreamReader rdr = new StreamReader(response.GetResponseStream()))
                {
                    result = rdr.ReadToEnd();
                }

                return new HttpSenderResponse(response.StatusCode, result);
            }
            catch (WebException ex)
            {
                HttpWebResponse webResponse = (HttpWebResponse)ex.Response;
                string result;
                using (StreamReader rdr = new StreamReader(webResponse.GetResponseStream()))
                {
                    result = rdr.ReadToEnd();
                }

                return new HttpSenderResponse(webResponse.StatusCode, result);
            }
        }

        public static GetScalarResponse GetScalar(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.KeepAlive = false;
            request.ProtocolVersion = HttpVersion.Version10;
            request.Method = "GET";

            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string result;
                using (StreamReader rdr = new StreamReader(response.GetResponseStream()))
                {
                    result = rdr.ReadToEnd();
                }

                return new GetScalarResponse(response.StatusCode, result);
            }
            catch (WebException ex)
            {
                HttpWebResponse webResponse = (HttpWebResponse)ex.Response;

                string result;
                using (StreamReader rdr = new StreamReader(webResponse.GetResponseStream()))
                {
                    result = rdr.ReadToEnd();
                }

                return new GetScalarResponse(webResponse.StatusCode, result);
            }
        }
    }
}