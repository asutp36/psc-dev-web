using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Web;

namespace MobileIntegration.Controllers.Supplies
{
    public class Sender
    {
        public static HttpResponse SendPost(string addres, string json, bool auth = false)
        {
            #region адреса различные
            // тест
            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://ptsv2.com/t/rq63q-1572107969/post");

            // первый сервис
            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.myeco24.ru/transactions/post/cash");

            // второй сервис
            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://eco.voodoolab.io/api/externaldb/user-create"); //new card
            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://eco.voodoolab.io/api/externaldb/set-waste"); //списание
            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://eco.voodoolab.io/api/externaldb/set-replenish"); //пополнение
            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://eco.voodoolab.io/api/externaldb/user-balance"); //узнать баланс
            #endregion 

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(addres);

            if (auth)
            {
                request.Host = "api.myeco24.ru";
                request.Headers.Add("Authorization", "Basic ZWMwMjRfQXBJOl9FY09fMl8yMl9XM2JfQHB8");
            }

            request.KeepAlive = false;
            request.PreAuthenticate = true;
            request.ProtocolVersion = HttpVersion.Version10;
            request.Method = "POST";

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
            catch(Exception e)
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

        public static HttpResponse Send(string address, string json)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("http://192.168.81.201:5000/");

                HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "qwerty");

                if (!string.IsNullOrEmpty(json))
                {
                    httpRequestMessage.Content = new StringContent(json, Encoding.UTF8, "application/json");
                }

                HttpResponseMessage result = client.SendAsync(httpRequestMessage).Result;

                HttpResponse response = new HttpResponse();
                response.StatusCode = result.StatusCode;
                response.ResultMessage = result.Content.ReadAsStringAsync().Result;

                return response;
            }
            catch (HttpRequestException e)
            {
                return new HttpResponse() { StatusCode = 0, ResultMessage = e.Message };
            }
            catch (AggregateException e)
            {
                HttpResponse r = new HttpResponse();
                r.StatusCode = (HttpStatusCode)0;

                foreach (Exception ex in e.InnerExceptions)
                {
                    r.ResultMessage += " " + ex.Message;
                }
                return r;
            }
            catch (SocketException e)
            {
                return new HttpResponse() { StatusCode = (HttpStatusCode)0, ResultMessage = "нет связи" };
            }
        }
    }
}