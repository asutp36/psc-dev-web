using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using NotificationService.Controllers.Supplies;

namespace NotificationService.Controllers
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

                    string result = WhattsAppSender.SendMessage(JsonConvert.SerializeObject(message)); ;

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
    }
}
