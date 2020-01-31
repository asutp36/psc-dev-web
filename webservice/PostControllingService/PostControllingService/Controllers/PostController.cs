using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using PostControllingService.Controllers.Supplies;

namespace PostControllingService.Controllers
{
    public class PostController : ApiController
    {
        [HttpPost]
        [ActionName("price")]
        public HttpResponseMessage SendPrice([FromBody]PricesChange change)
        {
            Logger.InitLogger();

            try
            {
                if (change != null)
                {
                    //string toLog = "Посты: ";
                    //foreach(int wash in change.posts)
                    //{
                    //    toLog += wash + ", ";
                    //}

                    //toLog += "\nЦены: ";
                    //foreach(int p in change.prices)
                    //{
                    //    toLog += p.Function + ": " + p.Value + "\n";
                    //}

                    //Logger.Log.Debug(String.Format("Получен запрос на изменение. \n{0}", toLog));



                    HttpWebResponse response = HttpSender.SendPrice(JsonConvert.SerializeObject(change)); ;

                    //ResponseSendMessage response = JsonConvert.DeserializeObject<ResponseSendMessage>(result);

                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        Logger.Log.Error("Пришёл плохой ответ");
                        //Logger.Log.Error(response.message + Environment.NewLine);

                        return Request.CreateResponse(HttpStatusCode.InternalServerError);
                    }
                    else
                    {
                        Logger.Log.Debug(String.Format("{0} : {1}", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"), "Отправлено успешно") + Environment.NewLine);

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
    }
}
