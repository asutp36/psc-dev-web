using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using NotificationService.Controllers.Supplies;
using NotificationService.Models;
using System.Data.Common;

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
                    Logger.Log.Debug(String.Format("SendMessage: запуск с парметрами:\nsender: {0}, receiver: {1}, body: {2}", msgToSend.sender, msgToSend.receiver, msgToSend.body));

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

                        DbCommand command = _model.Database.Connection.CreateCommand();

                        command.CommandText = "INSERT INTO NoticeHistory (Sender, DTimeReceive, IDNoticeRecipientsGroup, Message, IDNoticeStatus)" +
                                                $" VALUES(\'{msgToSend.sender}\', \'{DateTime.Now.ToString("yyyyMMdd HH:mm:ss.fff")}\', " +
                                                $"(select IDNoticeRecipientsGroup from NoticeRecipientsGroup where Code = \'{msgToSend.receiver}\'), \'{msgToSend.body}\', " +
                                                $"(select IDNoticeStatus from NoticeStatus where Code = \'received\'));" +
                                                " SELECT SCOPE_IDENTITY()";

                        var id = command.ExecuteScalar();
                        Logger.Log.Debug("SendMesssage: запись в историю добавлена. IDNoticeHistory = " + id);

                        MessageChatID message = new MessageChatID(recipient, msgToSend.body);
                        Logger.Log.Debug(String.Format("SendMessage: Отправка сообщения: chatId: {0}, body: {1}", message.chatId, message.body));
                        result = WhattsAppSender.SendMessage(JsonConvert.SerializeObject(message), "https://eu33.chat-api.com/instance27633/sendMessage?token=0qgid5wjmhb8vw7d");

                        ResponseSendMessage response = JsonConvert.DeserializeObject<ResponseSendMessage>(result);

                        if (!response.sent)
                        {
                            Logger.Log.Error(response.message + Environment.NewLine);
                            return Request.CreateResponse(HttpStatusCode.InternalServerError);
                        }
                        else
                        {
                            Logger.Log.Debug(String.Format("{0} : {1}", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"), response.message));

                            command.CommandText = "UPDATE NoticeHistory " +
                                $"SET DTimeSent = \'{DateTime.Now.ToString("yyyyMMdd HH:mm:ss.fff")}\', IDNoticeStatus = (select IDNoticeStatus from NoticeStatus where Code = \'sent\') " +
                                $"WHERE IDNoticeHistory = {id};";

                            command.ExecuteNonQuery();

                            Logger.Log.Debug("SendMessage: обновил запись (отправлено). id = " + id + Environment.NewLine);
                            return Request.CreateResponse(HttpStatusCode.OK);
                        }
                    }

                    Logger.Log.Error("База данных не найдена!" + Environment.NewLine);
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                }
                else
                {
                    Logger.Log.Error("message == null. Ошибка в данных запроса" + Environment.NewLine);
                    return Request.CreateResponse(HttpStatusCode.NoContent);
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        [ActionName("message")]
        public HttpResponseMessage SendMessage([FromBody]MessagePhone msg)
        {
            Logger.InitLogger();

            Logger.Log.Debug("SendMessage (from post): запуск с параметрами:\n" + JsonConvert.SerializeObject(msg));

            if (!msg.phone.Equals(""))
            {
                ResponseSendMessage resp = JsonConvert.DeserializeObject<ResponseSendMessage>(WhattsAppSender.SendMessage(JsonConvert.SerializeObject(msg), "https://eu33.chat-api.com/instance27633/sendMessage?token=0qgid5wjmhb8vw7d"));

                if (resp.sent)
                {
                    Logger.Log.Debug("SendMessage (from post): сообщение отправлено");
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                else
                {
                    Logger.Log.Debug("SendMessage (from post): сообщение не отправлено.\n" + resp.message);
                    return Request.CreateResponse((HttpStatusCode)424);
                }
            }

            Logger.Log.Error("SendMessage (from post): телефон пустой");
            return Request.CreateResponse(HttpStatusCode.NoContent);
        }
    }
}
