using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace HangFireTest.Controllers.Helpers
{
    public class HttpSender
    {
        public static HttpResponse SendGet(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.KeepAlive = false;
            request.ProtocolVersion = HttpVersion.Version10;
            request.Method = "GET";

            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return new HttpResponse
                    {
                        StatusCode = response.StatusCode,
                        ResultMessage = response.ToString()
                    };
                }
                else
                {
                    string result;
                    using (StreamReader rdr = new StreamReader(response.GetResponseStream()))
                    {
                        result = rdr.ReadToEnd();
                    }

                    return new HttpResponse
                    {
                        StatusCode = response.StatusCode,
                        ResultMessage = result
                    };
                }
            }
            catch (WebException ex)
            {
                if (ex.InnerException != null && ex.InnerException.InnerException != null && ex.InnerException.InnerException != null &&
                    typeof(SocketException) == ex.InnerException.InnerException.GetType())
                {
                    SocketException se = (SocketException)ex.InnerException.InnerException;

                    if (se.ErrorCode == 10060)
                        return new HttpResponse { StatusCode = 0 };

                    return new HttpResponse { ResultMessage = ex.Message };
                }
                HttpWebResponse webResponse = (HttpWebResponse)ex.Response;

                string result;
                using (StreamReader rdr = new StreamReader(webResponse.GetResponseStream()))
                {
                    result = rdr.ReadToEnd();
                }

                return new HttpResponse
                {
                    StatusCode = webResponse.StatusCode,
                    ResultMessage = result
                };
            }
        }
    }
}
