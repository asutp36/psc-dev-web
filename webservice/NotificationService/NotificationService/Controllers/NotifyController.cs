using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using NotificationService.Controllers.Supplies;
using NotificationService.Models;

namespace NotificationService.Controllers
{
    public class NotifyController : ApiController
    {
        ModelDb _model = new ModelDb();

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

                    if (_model.Database.Exists())
                    {
                        _model.Database.Connection.Open();
                        Logger.Log.Debug("SendMessage: Соединение с базой: " + _model.Database.Connection.State);

                        var prmRecipientCode = new System.Data.SqlClient.SqlParameter("@p_RecipientCode", System.Data.SqlDbType.NVarChar);
                        prmRecipientCode.Value = msgToSend.receiver;

                        string recipient = _model.Database
                            .SqlQuery<GetNoticeRecipient_Result>("GetNoticeRecipient @p_RecipientCode", prmRecipientCode)
                            .ToList().First().Recipient;


                        //MessagePhone message = new MessagePhone(recipient, msgToSend.body);
                        //Logger.Log.Debug(String.Format("Отправка сообщения по номеру телефона: phone: {0}, body: {1}", message.phone, message.body));
                        //result = WhattsAppSender.SendMessage(JsonConvert.SerializeObject(message), "https://eu33.chat-api.com/instance27633/sendMessage?token=0qgid5wjmhb8vw7d");

                
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

                    Logger.Log.Error("База данных не найдена!");
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
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
