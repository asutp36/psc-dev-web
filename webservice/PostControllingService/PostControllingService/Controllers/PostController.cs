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
        public HttpResponseMessage SendPrice([FromBody]ChangePricesData change)
        {
            Logger.InitLogger();

            try
            {
                if (change != null)
                {

                    foreach (Price p in change.prices)
                    {
                        Logger.Log.Debug("Изменение тарифа. Отправка на пост: " + p);
                        SendPriceResponse response = HttpSender.SendPost("http://109.196.164.28:5000/api/post/rate" , JsonConvert.SerializeObject(p));

                        if (response.StatusCode != HttpStatusCode.OK)
                        {
                            Logger.Log.Error(String.Format("Ответ сервера: {0}\n{1}", response.StatusCode, response.Result));

                            return Request.CreateResponse(HttpStatusCode.Conflict);
                        }
                    }

                    Logger.Log.Debug(String.Format("{0} : {1}", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"), "Отправлено успешно") + Environment.NewLine);

                    return Request.CreateResponse(HttpStatusCode.OK);
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

        [HttpPost]
        [ActionName("incrbalance")]
        public HttpResponseMessage IncreaseBalance([FromBody]IncreaseBalance balance)
        {
            Logger.InitLogger();

            try
            {
                if (balance != null)
                {
                    Logger.Log.Debug("Пополнение баланса. Отправка на пост: " + balance.ToString());
                    SendPriceResponse response = HttpSender.SendPost("http://109.196.164.28:5000/api/post/balance/increase", JsonConvert.SerializeObject(balance));

                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        Logger.Log.Error(String.Format("Ответ сервера: {0}\n{1}", response.StatusCode, response.Result));

                        return Request.CreateResponse(HttpStatusCode.Conflict);
                    }

                    Logger.Log.Debug(String.Format("{0} : {1}", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"), "Отправлено успешно") + Environment.NewLine);

                    return Request.CreateResponse(HttpStatusCode.OK);
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
