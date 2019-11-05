using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using WebService.Controllers.Supplies;

namespace WebService.Controllers
{
    public class NotifyController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage SendMessage([FromBody]Message message)
        {
            Logger.InitLogger();

            try
            {
                if (message != null)
                {
                    Logger.Log.Debug(String.Format("Получено сообщение: chatId - {0}; body - {1}", message.chatId, message.body));

                    string result = PostRequest(JsonConvert.SerializeObject(message));

                    ResponseSendMessage response = JsonConvert.DeserializeObject<ResponseSendMessage>(result);

                    if (!response.sent)
                    {
                        Logger.Log.Error(response.message + Environment.NewLine);

                        return Request.CreateResponse(HttpStatusCode.InternalServerError);
                    }
                    else
                    {
                        Logger.Log.Debug(String.Format("{0} : {1}", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"), response.message) + Environment.NewLine);

                        return Request.CreateResponse(HttpStatusCode.OK);
                    }

                }

                var responseBad = Request.CreateResponse(HttpStatusCode.NoContent);
                return responseBad;
            }
            catch (Exception e)
            {
                Logger.Log.Error(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);

                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        private string PostRequest(string json)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://eu33.chat-api.com/instance27633/sendMessage?token=0qgid5wjmhb8vw7d");
            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://ptsv2.com/t/rq63q-1572107969/post");
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

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string result;
            using (StreamReader rdr = new StreamReader(response.GetResponseStream()))
            {
                result = rdr.ReadToEnd();
            }

            return result;
        }
    }
}