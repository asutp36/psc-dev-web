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
        [ActionName("message")]
        public HttpResponseMessage SendMessage([FromBody]MessageToSend msgToSend)
        {
            Logger.InitLogger();
            try
            {
                if (msgToSend != null)
                {
                    Logger.Log.Debug(String.Format("SendMessage: запуск с парметрами:\nsender: {0},\n receiver: {1}, isPhone: {2},\nbody: {3}", msgToSend.sender, msgToSend.receiver, msgToSend.isPhone, msgToSend.body));

                    string result = "";
                    if (msgToSend.isPhone)
                    {
                        MessagePhone message = new MessagePhone(msgToSend.receiver, msgToSend.body);
                        Logger.Log.Debug(String.Format("Отправка сообщения по номеру телефона: phone: {0}, body: {1}", message.phone, message.body));
                        result = WhattsAppSender.SendMessage(JsonConvert.SerializeObject(message), "https://ptsv2.com/t/rq63q-1572107969/post");
                    }
                    else
                    {
                        MessageChatID message = new MessageChatID(msgToSend.receiver, msgToSend.body);
                        Logger.Log.Debug(String.Format("Отправка сообщения по chatId: chatId: {0}, body: {1}", message.chatId, message.body));
                        result = WhattsAppSender.SendMessage(JsonConvert.SerializeObject(message), "https://ptsv2.com/t/rq63q-1572107969/post");
                    }

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
                else
                {
                    Logger.Log.Error("message == null. Ошибка в данных запроса");
                    return Request.CreateResponse(HttpStatusCode.NoContent);
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }
    }
}
