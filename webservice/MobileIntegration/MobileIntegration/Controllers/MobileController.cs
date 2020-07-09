using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MobileIntegration.Models;
using MobileIntegration.Controllers.Supplies;
using System.Data.Common;
using Newtonsoft.Json;
using System.Data.SqlClient;

namespace MobileIntegration.Controllers
{
    public class MobileController : ApiController
    {
        private ModelDb _model = new ModelDb();

        [HttpPost]
        [ActionName("increase_dev")]
        //где записан баланс?
        //можно просто через операцию
        public HttpResponseMessage IncreaseBalanceDev([FromBody]IncreaseFromMobile increase)
        {
            Logger.InitLogger();
            if (increase != null)
            {
                try
                {
                    Logger.Log.Debug(String.Format("IncreaseBalnce. Запуск с параметрами:\n" +
                        "time_send: {0}, hash: {1}\ncard: {2}, value: {3}\nfrom: {4}, operation_type: {5}", increase.time_send, increase.hash, increase.card.ToString(),
                        increase.value, increase.from, increase.operation_type));

                    if (_model.Database.Exists())
                    {
                        _model.Database.Connection.Open();
                        Logger.Log.Debug("Db connection: " + _model.Database.Connection.State.ToString());

                        DbCommand commandBalance = _model.Database.Connection.CreateCommand();
                        commandBalance.CommandText = "select Balance " +
                            "from Operations " +
                            $"where DTime = (select max(DTime) from Operations where IDCard = (select IDCard from Cards where CardNum = {increase.card})) " +
                            $"and IDCard = (select IDCard from Cards where CardNum = {increase.card})";

                        DbCommand command = _model.Database.Connection.CreateCommand();
                        command.CommandText = "INSERT INTO Operations (IDCard, IDPsc, IDOperationType, DTime, Amount, Balance, LocalizedBy, LocalizedID)" +
                                                $" VALUES((select IDCard from Cards where CardNum =  {increase.card}), " +
                                                $"(select IDPsc from Psces where Name = 'MobileApp'), 2, \'{increase.time_send.ToString("yyyyMMdd HH:mm:ss")}\', {increase.value}," +
                                                $" ({commandBalance.CommandText}) + {increase.value}, -1, -1);" +
                                                " SELECT SCOPE_IDENTITY()";

                        Logger.Log.Debug("Command is: " + command.CommandText);

                        var id = command.ExecuteScalar();
                        Int32 serverID = Convert.ToInt32(id.ToString());
                        Logger.Log.Debug("Operation added serverID:" + serverID);

                        _model.Database.Connection.Close();

                        var responseGood = Request.CreateResponse(HttpStatusCode.OK);
                        //responseGood.Headers.Add("ServerID", serverID.ToString());
                        return responseGood;
                    }
                }
                catch (Exception e)
                {
                    Logger.Log.Error("IncreaseBalance reciever: " + e.Message.ToString());
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                }
            }

            return Request.CreateResponse(HttpStatusCode.NoContent);
        }

        [HttpPost]
        [ActionName("increase")]
        //где записан баланс?
        //можно просто через операцию
        public HttpResponseMessage IncreaseBalance([FromBody]IncreaseFromMobile increase)
        {
            Logger.InitLogger();
            if (increase != null)
            {
                try
                {
                    Logger.Log.Debug(String.Format("IncreaseBalnce. Запуск с параметрами:\n" +
                        "time_send: {0}, hash: {1}\ncard: {2}, value: {3}\nfrom: {4}, operation_type: {5}", increase.time_send, increase.hash, increase.card.ToString(),
                        increase.value, increase.from, increase.operation_type));
                    if (CryptHash.CheckHashCode(increase.hash, increase.time_send.ToString("yyyy-MM-dd HH:mm:ss")))
                    {
                        if (_model.Database.Exists())
                        {
                            _model.Database.Connection.Open();
                            Logger.Log.Debug("Db connection: " + _model.Database.Connection.State.ToString());

                            DbCommand commandBalance = _model.Database.Connection.CreateCommand();
                            commandBalance.CommandText = $"select " +
                                $"isnull(o.Balance, 0) " +
                                $"from Cards c " +
                                $"left join Operations o on o.IDCard = c.IDCard " +
                                $"and o.DTime = (select MAX(DTime) from Operations where IDCard = c.IDCard) " +
                                $"where c.CardNum = '{increase.card}'";

                            DbCommand command = _model.Database.Connection.CreateCommand();
                            command.CommandText = "INSERT INTO Operations (IDCard, IDPsc, IDOperationType, DTime, Amount, Balance, LocalizedBy, LocalizedID)" +
                                                    $" VALUES((select IDCard from Cards where CardNum = '{increase.card}'), " +
                                                    $"(select IDPsc from Psces where Name = 'MobileApp'), 2, \'{increase.time_send.ToString("yyyyMMdd HH:mm:ss")}\', {increase.value}," +
                                                    $" ({commandBalance.CommandText}) + {increase.value}, -1, -1);" +
                                                    " SELECT SCOPE_IDENTITY()";

                            Logger.Log.Debug("Command is: " + command.CommandText);

                            var id = command.ExecuteScalar();
                            Int32 serverID = Convert.ToInt32(id.ToString());
                            Logger.Log.Debug("Operation added serverID:" + serverID);

                            _model.Database.Connection.Close();

                            var responseGood = Request.CreateResponse(HttpStatusCode.OK);
                            //responseGood.Headers.Add("ServerID", serverID.ToString());
                            return responseGood;
                        }
                    }
                    else
                    {
                        Logger.Log.Error("Unauthorized" + Environment.NewLine);
                        return Request.CreateResponse(HttpStatusCode.Unauthorized);
                    }
                }
                catch (Exception e)
                {
                    Logger.Log.Error("IncreaseBalance reciever: " + e.Message.ToString() + Environment.NewLine);
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                }
            }

            return Request.CreateResponse(HttpStatusCode.NoContent);
        }

        [HttpPost]
        [ActionName("get_balance")]
        //как высчитывать баланс карты?
        public HttpResponseMessage GetBalance([FromBody]GetBalanceFromMobile getBalance)
        {
            Logger.InitLogger();

            if (getBalance != null)
            {
                try
                {
                    Logger.Log.Debug(String.Format("Запуск с параметрами: номер карты: {0}", getBalance.card));

                    if (_model.Database.Exists())
                    {
                        _model.Database.Connection.Open();
                        Logger.Log.Debug("Db connection: " + _model.Database.Connection.State.ToString());

                        var prmCard = new System.Data.SqlClient.SqlParameter("@CardNum", System.Data.SqlDbType.Int);
                        prmCard.Value = getBalance.card;

                        DbCommand command = _model.Database.Connection.CreateCommand();
                        command.CommandText = "select Balance " +
                            "from Operations " +
                            $"where DTime = (select max(DTime) from Operations where IDCard = (select IDCard from Cards where CardNum = {getBalance.card})) " +
                            $"and IDCard = (select IDCard from Cards where CardNum = {getBalance.card})";

                        var balance = command.ExecuteScalar();

                        var response = Request.CreateResponse();
                        response.Headers.Add("Card", prmCard.Value.ToString());

                        if (balance != null)
                        {
                            response.StatusCode = HttpStatusCode.OK;
                            response.Headers.Add("Balance", balance.ToString());
                        }
                        else
                        {
                            response.StatusCode = HttpStatusCode.NotFound;
                            response.Headers.Add("Message", "Card not found");
                        }

                        _model.Database.Connection.Close();

                        return response;
                    }

                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                }
                catch (Exception e)
                {
                    Logger.Log.Error("GetBalance: " + e.Message.ToString());
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                }
            }

            return Request.CreateResponse(HttpStatusCode.NoContent);
        }

        //[HttpPost]
        //[ActionName("change_cardstate")]
        //public HttpResponseMessage ChangeCardState()
        //{
        //    Logger.InitLogger();
        //    try
        //    {

        //    }
        //    catch (Exception e)
        //    {
        //        Logger.Log.Error("ChangCardState reciever: " + e.Message.ToString());
        //        return Request.CreateResponse(HttpStatusCode.InternalServerError);
        //    }
        //}

        [HttpPost]
        [ActionName("change_phone")]
        //что делать с localized?
        public HttpResponseMessage ChangePhone([FromBody]ChangePhoneFromMobile change)
        {
            Logger.InitLogger();
            if (change != null)
            {
                try
                {
                    Logger.Log.Debug(String.Format("Запуск с параметрами: старый номер: {0}, новый: {1}", change.Old, change.New));

                    if (_model.Database.Exists())
                    {
                        _model.Database.Connection.Open();
                        Logger.Log.Debug("Db connection: " + _model.Database.Connection.State.ToString());

                        DbCommand command = _model.Database.Connection.CreateCommand();

                        command.CommandText = $"UPDATE Owners SET Phone = '{change.New}' WHERE IDOwner = (SELECT IDOwner FROM Owners WHERE Phone = '{change.Old}')";
                        Logger.Log.Debug("Command is: " + command.CommandText);

                        var id = command.ExecuteScalar();

                        return Request.CreateResponse(HttpStatusCode.OK);
                    }

                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "База данных не найдена");
                }
                catch (Exception e)
                {
                    Logger.Log.Error("ChangePhone reciever: " + e.Message.ToString());
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                }
            }

            return Request.CreateResponse(HttpStatusCode.NoContent);
        }

        [HttpPost]
        [ActionName("start_post")]
        public HttpResponseMessage StartPost([FromBody]StartPostBindingModel model)
        {
            Logger.InitLogger();

            if (model != null)
            {
                Logger.Log.Debug("StrartPost: Запуск с параметрами:\n" + JsonConvert.SerializeObject(model));

                try
                {
                    if (CryptHash.CheckHashCode(model.hash, model.time_send.ToString("yyyy-MM-dd HH:mm:ss")))
                    {
                        var card = _model.Cards.Where(c => c.CardNum == model.card).FirstOrDefault();
                        if (card != null)
                        {
                            if (model.balance > 50)
                            {
                                var post = _model.Posts.Where(p => p.Code == model.post).FirstOrDefault();

                                if (post != null)
                                {
                                    if (post.Code == "М202-2")
                                    {
                                        Logger.Log.Error(String.Format("StartPost: Post {0} is busy", post.Code) + Environment.NewLine);
                                        return Request.CreateResponse((HttpStatusCode)423);
                                    }

                                    Logger.Log.Debug(String.Format("StartPost: Starting post {0}", post.Code) + Environment.NewLine);
                                    return Request.CreateResponse(HttpStatusCode.OK);
                                }

                                Logger.Log.Error("StartPost: Post not found" + Environment.NewLine);
                                return Request.CreateResponse(HttpStatusCode.NotFound);
                            }

                            Logger.Log.Error("StartPost: Balance is weak" + Environment.NewLine);
                            return Request.CreateResponse((HttpStatusCode)422);
                        }

                        Logger.Log.Error("StartPost: Card not found" + Environment.NewLine);
                        return Request.CreateResponse(HttpStatusCode.Forbidden);
                    }

                    Logger.Log.Error("StartPost: Unauthorized" + Environment.NewLine);
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
                }
                catch(Exception e)
                {
                    Logger.Log.Error("StartPost: " + e.Message.ToString() + Environment.NewLine);
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                }
            }

            return Request.CreateResponse(HttpStatusCode.NoContent);
        }

        [HttpPost]
        [ActionName("start_post-dev")]
        public HttpResponseMessage StartPostDev([FromBody]StartPostBindingModel model)
        {
            Logger.InitLogger();

            if (model != null)
            {
                Logger.Log.Debug("StrartPost: Запуск с параметрами:\n" + JsonConvert.SerializeObject(model));

                try
                {
                    if (true)
                    {
                        var card = _model.Cards.Where(c => c.CardNum == model.card).FirstOrDefault();
                        if (card != null)
                        {
                            if (model.balance > 50)
                            {
                                var post = _model.Posts.Where(p => p.Code == model.post).FirstOrDefault();

                                if (post != null)
                                {
                                    if (post.Code == "М202-2")
                                    {
                                        Logger.Log.Error(String.Format("StartPost: Post {0} is busy", post.Code) + Environment.NewLine);
                                        return Request.CreateResponse((HttpStatusCode)423);
                                    }

                                    Logger.Log.Debug(String.Format("StartPost: Starting post {0}", post.Code) + Environment.NewLine);
                                    return Request.CreateResponse(HttpStatusCode.OK);
                                }

                                Logger.Log.Error("StartPost: Post not found" + Environment.NewLine);
                                return Request.CreateResponse(HttpStatusCode.NotFound);
                            }

                            Logger.Log.Error("StartPost: Balance is weak" + Environment.NewLine);
                            return Request.CreateResponse((HttpStatusCode)422);
                        }

                        Logger.Log.Error("StartPost: Card not found" + Environment.NewLine);
                        return Request.CreateResponse(HttpStatusCode.Forbidden);
                    }

                    Logger.Log.Error("StartPost: Unauthorized" + Environment.NewLine);
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
                }
                catch (Exception e)
                {
                    Logger.Log.Error("StartPost: " + e.Message.ToString() + Environment.NewLine);
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                }
            }

            return Request.CreateResponse(HttpStatusCode.NoContent);
        }

        [HttpPost]
        [ActionName("stop_post-dev")]
        public HttpResponseMessage StopPostDev([FromBody]StartPostBindingModel model)
        {
            Logger.InitLogger();
            Logger.Log.Debug($"StopPost: отправка списания по карте {model.card}");

            string resp = Sender.SendPost("http://loyalty.myeco24.ru/api/externaldb/set-waste", JsonConvert.SerializeObject(new Decrease(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), model.card, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "1", 80)));

            Logger.Log.Debug($"StopPost: Ответ от их сервера: {resp}");

            return Request.CreateErrorResponse(HttpStatusCode.Created, resp);
        }


        [HttpPost]
        [ActionName("new_card")]
        public HttpResponseMessage NewCard([FromBody]NewCard newCard)
        {
            Logger.InitLogger();

            if (newCard != null)
            {
                try
                {
                    Logger.Log.Debug(String.Format("NewCard: Запуск с параметрами: номер телефона: {0}", newCard.phone));

                    if (CryptHash.CheckHashCode(newCard.hash, newCard.time_send.ToString("yyyy-MM-dd HH:mm:ss")))
                    {
                        if (_model.Database.Exists())
                        {
                            _model.Database.Connection.Open();
                            Logger.Log.Debug("Db connection: " + _model.Database.Connection.State.ToString());

                            List<string> cards = GetCardsByPhone(newCard.phone);
                            if (cards.Count > 0)
                            {
                                _model.Database.Connection.Close();
                                Logger.Log.Error("NewCard: У пользователя есть карта" + Environment.NewLine);
                                return Request.CreateErrorResponse(HttpStatusCode.Conflict, new Exception("У пользователя уже есть карта"));
                            }

                            DbCommand command = _model.Database.Connection.CreateCommand();
                            command.CommandText = "select " +
                                "min(v.Num) " +
                                "from NumsMobileCards v " +
                                "left join Cards c on c.CardNum = v.Num " +
                                "where c.CardNum is null";

                            var cardNum = command.ExecuteScalar();

                            DbTransaction tran = _model.Database.Connection.BeginTransaction();
                            command.Transaction = tran;

                            try
                            {
                                command.CommandText = $"insert into Owners (Phone, LocalizedBy, LocalizedID) values ('{newCard.phone}', 0, 0)";
                                command.ExecuteNonQuery();
                                command.CommandText = $"insert into Cards (IDOwner, CardNum,  IDCardStatus, IDCardType, LocalizedBy, LocalizedID) values (scope_identity(), '{cardNum}', 1, 4, 0, 0)";
                                command.ExecuteNonQuery();

                                Logger.Log.Debug($"NewCard: добавлены Owner и Card. CardNum = {cardNum.ToString()}" + Environment.NewLine);
                                tran.Commit();
                            }
                            catch (Exception e)
                            {
                                Logger.Log.Error("NewCard: ошибка транзакции.\n" + e.Message + Environment.NewLine);
                                tran.Rollback();
                                _model.Database.Connection.Close();

                                return Request.CreateResponse(HttpStatusCode.InternalServerError);
                            }

                            var response = Request.CreateResponse();

                            response.StatusCode = HttpStatusCode.OK;
                            response.Headers.Add("CardNum", cardNum.ToString());

                            _model.Database.Connection.Close();

                            return response;
                        }
                        return Request.CreateResponse(HttpStatusCode.InternalServerError);
                    }

                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
                }
                catch (Exception e)
                {
                    Logger.Log.Error("NewCard: " + e.Message.ToString() + Environment.NewLine + e.StackTrace + Environment.NewLine);
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                }
            }

            return Request.CreateResponse(HttpStatusCode.NoContent);
        }

        private List<string> GetCardsByPhone(string phone)
        {
            List<string> cards = _model.Cards.Where(c => c.IDOwner == _model.Owners.Where(o => o.Phone.Equals(phone)).FirstOrDefault().IDOwner).Select(c => c.CardNum).ToList();

            return cards;
        }
    }
}
