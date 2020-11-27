using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace OrangeDataKlient.Controllers.Supplies
{
    public class HttpSender
    {
        public static HttpResponse SendPost(string addres, string json, string signature)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(addres);
            X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);

            X509Certificate2Collection collection = store.Certificates.Find(X509FindType.FindBySubjectName, "client", true);

            X509Certificate2 certificate = new X509Certificate2("client.crt");
            //request.ClientCertificates.Add(certificate);
            request.ClientCertificates = collection;
            request.KeepAlive = false;
            request.PreAuthenticate = true;
            request.ProtocolVersion = HttpVersion.Version10;
            request.Method = "POST";
            request.Headers.Add("X-Signature", signature);

            byte[] postBytes = Encoding.UTF8.GetBytes(json);

            request.ContentType = "application/json";
            request.Accept = "application/json";
            request.ContentLength = postBytes.Length;

            try
            {
                Stream requestStream = request.GetRequestStream();

                requestStream.Write(postBytes, 0, postBytes.Length);
                requestStream.Close();
            }
            catch (Exception e)
            {
                return new HttpResponse
                {
                    ResultMessage = e.Message
                };
            }

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
