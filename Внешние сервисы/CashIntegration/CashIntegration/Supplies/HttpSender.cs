using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CashIntegration.Supplies;
using Newtonsoft.Json;

namespace CashIntegration.Supplies
{
    class HttpSender
    {
        public static IntegrationResponse SendCash(string addres, string json, bool auth = false)
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
                request.Headers.Add("Authorization", "Basic X0VDMDI0X19AcElfOl9ARWNPM19fMjcwMV9XNXlfcF98X18=");
            }

            request.KeepAlive = false;
            request.PreAuthenticate = true;
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
                string result;
                using (StreamReader rdr = new StreamReader(response.GetResponseStream()))
                {
                    result = rdr.ReadToEnd();
                }
                IntegrationResponse res = JsonConvert.DeserializeObject<IntegrationResponse>(result);
                res.StatusCode = 200;
                return res;
            }
            catch (WebException ex)
            {
                HttpWebResponse webResponse = (HttpWebResponse)ex.Response;

                string result;
                using (StreamReader rdr = new StreamReader(webResponse.GetResponseStream()))
                {
                    result = rdr.ReadToEnd();
                }
                IntegrationResponse res = JsonConvert.DeserializeObject<IntegrationResponse>(result);
                return res;
            }
        }
    }
}
