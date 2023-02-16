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
using Microsoft.Ajax.Utilities;
using log4net.Repository;
using System.Web.WebPages;

namespace MobileIntegration.Controllers
{
    public class MobileController : ApiController
    {
        private ModelDb _model = new ModelDb();

        /// <summary>
        /// Пополнить баланс карты (хэш не проверяется)
        /// </summary>
        /// <param name="increase">Данные поплнения (хэн не обязательно)</param>
        /// <returns></returns>
        /// <response code="200">Баланс пополнен</response>
        /// <response code="500">Произошла ошибка</response>
        /// <response code="204">Некорректное входное значение</response>
        /// /// <response code="513">Ошибка при работе с базой данных</response>
        [HttpPost]
        [ActionName("increase_dev")]
        public HttpResponseMessage IncreaseBalanceDev([FromBody] IncreaseFromChanger increase)
        {
            Logger.InitLogger();
            if (increase != null)
            {
                try
                {
                    Logger.Log.Debug(String.Format("IncreaseBalnce. Запуск с параметрами:\n" +
                        "time_send: {0}, hash: {1}\ncard: {2}, value: {3}\nfrom: {4}, operation_type: {5}", increase.time_send, increase.hash, increase.card.ToString(),
                        increase.value, increase.fromCode, increase.operation_type));

                    if (_model.Database.Exists())
                    {
                        _model.Database.Connection.Open();
                        Logger.Log.Debug("Db connection: " + _model.Database.Connection.State.ToString());

                        DbCommand commandBalance = _model.Database.Connection.CreateCommand();
                        commandBalance.CommandText = $"select top 1 " +
                            $"isnull(o.Balance, 0) " +
                            $"from Cards c " +
                            $"left join Operations o on o.IDCard = c.IDCard " +
                            $"and o.DTime = (select MAX(DTime) from Operations where IDCard = c.IDCard) " +
                            $"where c.CardNum = '{increase.card}' " +
                            $"order by o.IDOperation desc";

                        DbCommand command = _model.Database.Connection.CreateCommand();
                        DbTransaction tran = _model.Database.Connection.BeginTransaction();
                        command.Transaction = tran;

                        try
                        {
                            command.CommandText = $"UPDATE Cards SET Balance = ({commandBalance.CommandText}) + {increase.value} WHERE CardNum = '{increase.card}'";
                            Logger.Log.Debug("IncreaseBalance: command is: " + command.CommandText);
                            command.ExecuteNonQuery();

                            command.CommandText = $"INSERT INTO Operations (IDDevice, IDOperationType, IDCard, DTime, Amount, Balance, LocalizedBy, LocalizedID) " +
                            $"VALUES ((select IDDevice from Device where Code = 'MOB-EM'), (select IDOperationType from OperationTypes where Code = 'increase'), " +
                            $"(select min(IDCard) as IDCard from Cards where CardNum = '{increase.card}'), '{increase.time_send.ToString("yyyy-MM-dd HH:mm:ss")}', {increase.value}, ({commandBalance.CommandText}) + {increase.value}, " +
                            $"(select IDDevice from Device where Code = 'MOB-EM'), 0);";
                            Logger.Log.Debug("IncreaseBalance: command is: " + command.CommandText);
                            command.ExecuteNonQuery();

                            tran.Commit();

                            command.CommandText = "SELECT SCOPE_IDENTITY();";

                            var serverID = command.ExecuteScalar();
                            Logger.Log.Debug("IncreaseBalance: операция добавлена. id = " + serverID.ToString() + Environment.NewLine);
                            _model.Database.Connection.Close();

                            _model.Database.Connection.Close();
                        }
                        catch (Exception e)
                        {
                            if (_model.Database.Connection.State == System.Data.ConnectionState.Open)
                            {
                                tran.Rollback();
                                _model.Database.Connection.Close();
                            }

                            Logger.Log.Error("IncreaseBalance: ошибка записи в базу\n" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                            return Request.CreateResponse((HttpStatusCode)513, "Ошибка при записи в базу");
                        }

                        int washID = _model.Changers.Where(c => c.IDDevice == _model.Device.Where(d => d.Code.Equals(increase.fromCode)).FirstOrDefault().IDDevice).FirstOrDefault().IDWash;
                        string washCode = _model.Wash.Find(washID).Code;


                        // отправка пополнения на пост
                        HttpResponse resp = Sender.SendPost("http://loyalty.myeco24.ru/api/externaldb/set-replenish", JsonConvert.SerializeObject(new Increase
                        {
                            time_send = increase.time_send.ToString("yyyy-MM-dd HH:mm:ss"),
                            hash = CryptHash.GetHashCode(increase.time_send.ToString("yyyy-MM-dd HH:mm:ss")),
                            card = increase.card,
                            value = increase.value,
                            wash_id = washCode,
                            operation_time = increase.time_send.ToString("yyyy-MM-dd HH:mm:ss")
                        }));

                        Logger.Log.Debug("IncreaseDev: отправлено пополнение. Ответ сервера:" + JsonConvert.SerializeObject(resp) + Environment.NewLine);


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

        /// <summary>
        /// Пополнить баланс карты
        /// </summary>
        /// <param name="increase">Данные поплнения</param>
        /// <returns></returns>
        /// <response code="200">Баланс пополнен</response>
        /// <response code="500">Произошла ошибка</response>
        /// <response code="204">Некорректное входное значение</response>
        /// <response code="513">Ошибка при работе с базой данных</response>
        [HttpPost]
        [ActionName("increase")]
        public HttpResponseMessage IncreaseBalance([FromBody] IncreaseFromMobile increase)
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
                            //Logger.Log.Debug("Db connection: " + _model.Database.Connection.State.ToString());

                            DbCommand commandBalance = _model.Database.Connection.CreateCommand();
                            commandBalance.CommandText = $"select top 1 " +
                            $"isnull(o.Balance, 0) " +
                            $"from Cards c " +
                            $"left join Operations o on o.IDCard = c.IDCard " +
                            $"and o.DTime = (select MAX(DTime) from Operations where IDCard = c.IDCard) " +
                            $"where c.CardNum = '{increase.card}' " +
                            $"order by o.IDOperation desc";

                            DbCommand command = _model.Database.Connection.CreateCommand();
                            DbTransaction tran = _model.Database.Connection.BeginTransaction();
                            command.Transaction = tran;

                            try
                            {
                                command.CommandText = $"UPDATE Cards SET Balance = ({commandBalance.CommandText}) + {increase.value} WHERE CardNum = '{increase.card}'";
                                //Logger.Log.Debug("IncreaseBalance: command is: " + command.CommandText);
                                command.ExecuteNonQuery();

                                command.CommandText = $"INSERT INTO Operations (IDDevice, IDOperationType, IDCard, DTime, Amount, Balance, LocalizedBy, LocalizedID) " +
                                $"VALUES ((select IDDevice from Device where Code = 'MOB-EM'), (select IDOperationType from OperationTypes where Code = 'increase'), " +
                                $"(select min(IDCard) as IDCard from Cards where CardNum = '{increase.card}'), '{increase.time_send.ToString("yyyy-MM-dd HH:mm:ss")}', {increase.value}, ({commandBalance.CommandText}) + {increase.value}, " +
                                $"(select IDDevice from Device where Code = 'MOB-EM'), 0);";
                                //Logger.Log.Debug("IncreaseBalance: command is: " + command.CommandText);
                                command.ExecuteNonQuery();

                                tran.Commit();

                                command.CommandText = "SELECT SCOPE_IDENTITY();";

                                var serverID = command.ExecuteScalar();
                                Logger.Log.Debug("IncreaseBalance: операция добавлена. id = " + serverID.ToString() + Environment.NewLine);
                                _model.Database.Connection.Close();

                                _model.Database.Connection.Close();
                            }
                            catch (Exception e)
                            {
                                if (_model.Database.Connection.State == System.Data.ConnectionState.Open)
                                {
                                    tran.Rollback();
                                    _model.Database.Connection.Close();
                                }

                                Logger.Log.Error("IncreaseBalance: ошибка записи в базу\n" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                                return Request.CreateResponse((HttpStatusCode)513, "Ошибка при записи в базу");
                            }
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

        /// <summary>
        /// Узнать баланс карты
        /// </summary>
        /// <param name="getBalance">Номер карты(int)</param>
        /// <returns>Заголовки: Card - номер карты, Balance - её баланс (если удачно отработал метод), Message - сообщение об ошибке (если неудачно)</returns>
        /// <response code="200">Удачно</response>
        /// <response code="404">Карта не найдена</response>
        /// <response code="500">Внутренняя ошибка</response>
        /// <response code="204">Некорректные входные данные</response>
        /// <response code="513">Ошибка при работе с базой данных</response>
        [HttpPost]
        [ActionName("get_balance")]
        public HttpResponseMessage GetBalance([FromBody] GetBalanceFromMobile getBalance)
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
                        command.CommandText = $"select top 1 " +
                            $"isnull(o.Balance, 0) " +
                            $"from Cards c " +
                            $"left join Operations o on o.IDCard = c.IDCard " +
                            $"and o.DTime = (select MAX(DTime) from Operations where IDCard = c.IDCard) " +
                            $"where c.CardNum = '{getBalance.card}' " +
                            $"order by o.IDOperation desc";

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

                    return Request.CreateResponse((HttpStatusCode)513, "База данных не найдена");
                }
                catch (Exception e)
                {
                    Logger.Log.Error("GetBalance: " + e.Message.ToString());
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                }
            }

            return Request.CreateResponse(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Узнать баланс карты
        /// </summary>
        /// <param name="getBalance">Номер карты(string)</param>
        /// <returns>Заголовки: Card - номер карты, Balance - её баланс (если удачно отработал метод), LastOperation - время посленй операции, Message - сообщение об ошибке (если неудачно)</returns>
        /// <response code="200"">Удачно</response>
        /// <response code="404">Карта не найдена</response>
        /// <response code="500">Внутренняя ошибка</response>
        /// <response code="204">Некорректные входные данные</response>
        [HttpPost]
        [ActionName("get_balance-dev")]
        public HttpResponseMessage GetBalanceDev([FromBody] GetBalance getBalance)
        {
            Logger.InitLogger();

            if (getBalance != null)
            {
                try
                {
                    Logger.Log.Debug(String.Format("GeyBalanceDev: Запуск с параметрами: номер карты: {0}", getBalance.card));

                    if (!CardIsExists(getBalance.card))
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Карта с таким номером не найдена");
                    }

                    if (_model.Database.Exists())
                    {
                        _model.Database.Connection.Open();
                        Logger.Log.Debug("Db connection: " + _model.Database.Connection.State.ToString());

                        DbCommand command = _model.Database.Connection.CreateCommand();
                        command.CommandText = $"select top 1 " +
                            $"isnull(o.Balance, 0) " +
                            $"from Cards c " +
                            $"left join Operations o on o.IDCard = c.IDCard " +
                            $"and o.DTime = (select MAX(DTime) from Operations where IDCard = c.IDCard) " +
                            $"where c.CardNum = '{getBalance.card}' " +
                            $"order by o.IDOperation desc";


                        var balance = command.ExecuteScalar();

                        var response = Request.CreateResponse();
                        response.Headers.Add("Card", getBalance.card);

                        command.CommandText = $"select MAX(DTime) from Operations o join Cards c on c.IDCard = o.IDCard where c.CardNum = '{getBalance.card}'";
                        DateTime lastOperation = (DateTime)command.ExecuteScalar();

                        if (balance != null)
                        {
                            Logger.Log.Debug("GetBalanceDev: Balance: " + balance.ToString() + Environment.NewLine);
                            response.StatusCode = HttpStatusCode.OK;
                            response.Headers.Add("Balance", balance.ToString());
                            response.Headers.Add("LastOperation", lastOperation.ToString("yyyy-MM-dd HH:mm:ss"));
                        }
                        else
                        {
                            Logger.Log.Error("GetBalance: карта не найдена" + Environment.NewLine);
                            response.StatusCode = HttpStatusCode.NotFound;
                            response.Headers.Add("Message", "Card not found");
                        }

                        _model.Database.Connection.Close();

                        return response;
                    }

                    return Request.CreateResponse((HttpStatusCode)513, "База данных не найдена");
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

        /// <summary>
        /// Изменить телефон владельца
        /// </summary>
        /// <param name="change">Старый и новый телефон</param>
        /// <returns></returns>
        /// <response code="200">Удачно</response>
        /// <response code="500">Внутренняя ошибка</response>
        /// <response code="204">Некорректные входные данные</response>
        [HttpPost]
        [ActionName("change_phone")]
        //что делать с localized?
        public HttpResponseMessage ChangePhone([FromBody] ChangePhoneFromMobile change)
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

        /// <summary>
        /// Начать мойку
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <response code="200">Удачно</response>
        /// <response code="404">Пост не найден</response>
        /// <response code="500">Внутренняя ошибка</response>
        /// <response code="204">Некорректные входные данные</response>
        /// <response code="424">Нет связи с постом</response>
        /// <response code="423">Пост занят</response>
        /// <response code="422">Меленький баланс</response>
        /// <response code="403">Карта не найдена</response>
        /// <response code="401">Хэш не прошёл проверку</response>
        /// <response code="513">Ошибка при работе с базой данных</response>
        [HttpPost]
        [ActionName("start_post")]
        public HttpResponseMessage StartPost([FromBody] StartPostBindingModel model)
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
                        if (card == null)
                        {
                            Logger.Log.Error("StartPost: Card not found" + Environment.NewLine);
                            Logger.Log.Info("StartPost: Вызов добавления новой карты" + Environment.NewLine);
                            var newCardResponseMessage = NewCard(new Supplies.NewCard
                            {
                                time_send = model.time_send.ToString("yyyy-MM-dd HH:mm:ss"),
                                hash = model.hash,
                                card = model.card,
                                phone = model.card
                            });

                            if (newCardResponseMessage.IsSuccessStatusCode)
                            {
                                Logger.Log.Info("StartPost: карта успешно добавлена" + Environment.NewLine);
                                card = _model.Cards.Where(c => c.CardNum == model.card).FirstOrDefault();
                            }
                            else 
                            {
                                Logger.Log.Info("StartPost: не удалось добавить карту" + Environment.NewLine);
                                return Request.CreateResponse(HttpStatusCode.Unauthorized);
                            }

                            //return Request.CreateResponse(HttpStatusCode.Forbidden);
                        }

                        if (card != null)
                        {
                            if (model.balance > 1)
                            {
                                int amount = model.balance;

                                string ip = GetPostIp(model.post);

                                if (ip == null || ip.Equals(""))
                                {
                                    Logger.Log.Error("StartPost: Post not found" + Environment.NewLine);
                                    return Request.CreateResponse(HttpStatusCode.NotFound);
                                }

                                Logger.Log.Debug("StartPost: запуск настоящего поста");

                                HttpResponse resp = Sender.SendPost("http://" + ip + "/api/post/balance/increase/card", JsonConvert.SerializeObject(new StartPostDevModel
                                {
                                    Amount = amount,
                                    Dtime = model.time_send.ToString("yyyy-MM-dd HH:mm:ss"),
                                    CardNum = model.card
                                }));

                                if (resp.StatusCode == 0)
                                {
                                    Logger.Log.Error("StartPost: Не удалось подключиться" + Environment.NewLine);
                                    return Request.CreateResponse((HttpStatusCode)424);
                                }

                                if (resp.StatusCode == (HttpStatusCode)423)
                                {
                                    Logger.Log.Error(String.Format("StartPost: Post {0} is busy", model.post) + Environment.NewLine);
                                    return Request.CreateResponse((HttpStatusCode)423);
                                }

                                try
                                {
                                    int updateResult = UpdateDTimeStartMoobileSendings(model);

                                    if (updateResult == 0)
                                    {
                                        int insertResult = InsertToMobileSendings(model);
                                        Logger.Log.Debug("Добавлена запись в MobileSendings: " + insertResult.ToString());
                                    }
                                    else
                                    {
                                        Logger.Log.Debug($"Обновлено время старта у {updateResult} записей");
                                    }
                                }
                                catch (Exception e)
                                {
                                    Logger.Log.Error("Ошибка при записи в журнал отправок в приложение: " + e.Message);
                                }

                                Logger.Log.Debug("Результат: " + resp.StatusCode);
                                return Request.CreateResponse(resp.StatusCode);
                            }

                            Logger.Log.Error("StartPost: Balance is weak" + Environment.NewLine);
                            return Request.CreateResponse((HttpStatusCode)422);
                        }
                    }

                    Logger.Log.Error("StartPost: Unauthorized" + Environment.NewLine);
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
                }
                catch (Exception e)
                {
                    Logger.Log.Error("StartPost: " + e.Message.ToString() + Environment.NewLine + e.StackTrace.ToString() + Environment.NewLine);
                    if (e.InnerException != null)
                    {
                        Logger.Log.Error("StartPost: InnerException: " + e.InnerException.Message.ToString() + Environment.NewLine);
                    }

                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                }
            }

            return Request.CreateResponse(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Начать мойку (хэш не проверяется)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <response code="200">Удачно</response>
        /// <response code="404">Пост не найден</response>
        /// <response code="500">Внутренняя ошибка</response>
        /// <response code="204">Некорректные входные данные</response>
        /// <response code="424">Нет связи с постом</response>
        /// <response code="423">Пост занят</response>
        /// <response code="422">Меленький баланс</response>
        /// <response code="403">Карта не найдена</response>
        /// <response code="513">Ошибка при работе с базой данных</response>
        [HttpPost]
        [ActionName("start_post-dev")]
        public HttpResponseMessage StartPostDev([FromBody] StartPostBindingModel model)
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
                                int amount = model.balance;

                                string ip = GetPostIp(model.post);

                                if (ip == null || ip.Equals(""))
                                {
                                    Logger.Log.Error("StartPost: Post not found" + Environment.NewLine);
                                    return Request.CreateResponse(HttpStatusCode.NotFound);
                                }

                                Logger.Log.Debug("StartPost: запуск настоящего поста");
                                HttpResponse resp = Sender.SendPost("http://" + ip + "/api/post/balance/increase/card", JsonConvert.SerializeObject(new StartPostDevModel
                                {
                                    Amount = amount,
                                    Dtime = model.time_send.ToString("yyyy-MM-dd HH:mm:ss"),
                                    CardNum = model.card
                                }));

                                if (resp.StatusCode == 0)
                                {
                                    Logger.Log.Error("StartPost: Не удалось подключиться" + Environment.NewLine);
                                    return Request.CreateResponse((HttpStatusCode)424);
                                }

                                if (resp.StatusCode == (HttpStatusCode)423)
                                {
                                    Logger.Log.Error(String.Format("StartPost: Post {0} is busy", model.post) + Environment.NewLine);
                                    return Request.CreateResponse((HttpStatusCode)423);
                                }

                                try
                                {
                                    int updateResult = UpdateDTimeStartMoobileSendings(model);
                                    
                                    if (updateResult == 0)
                                    {
                                        int insertResult = InsertToMobileSendings(model);
                                        Logger.Log.Debug("Добавлена запись в MobileSendings: " + insertResult.ToString());
                                    }
                                    else
                                    {
                                        Logger.Log.Debug($"Обновлено время страта у {updateResult} записей");
                                    }                                    
                                }
                                catch(Exception e)
                                {
                                    Logger.Log.Error("Ошибка при записи в журнал отправок в приложение: " + e.Message);
                                }

                                Logger.Log.Debug("Результат: " + resp.StatusCode);
                                return Request.CreateResponse(resp.StatusCode);
                            }

                            Logger.Log.Error("StartPost: Balance is weak" + Environment.NewLine);
                            return Request.CreateResponse((HttpStatusCode)422);
                        }

                        Logger.Log.Error("StartPost: Card not found" + Environment.NewLine);
                        return Request.CreateResponse(HttpStatusCode.Forbidden);
                    }
                }
                catch (Exception e)
                {
                    Logger.Log.Error("StartPost: " + e.GetType().ToString() + Environment.NewLine + e.Message.ToString() + Environment.NewLine + e.StackTrace.ToString() + Environment.NewLine);
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                }
            }

            return Request.CreateResponse(HttpStatusCode.NoContent);
        }

        private int UpdateDTimeStartMoobileSendings(StartPostBindingModel start)
        {
            try
            {
                _model.Database.Connection.Open();

                DbCommand command = _model.Database.Connection.CreateCommand();
                command.CommandText = $@"update MobileSendings 
                set IDPost = (select IDPost from Posts where QRCode = '{start.post}'), DTimeStart = '{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}' 
                where IDCard = (select IDCard from Cards where CardNum = '22') 
                and DTimeEnd is null";

                var result = command.ExecuteScalar();
                if (result == null)
                    return 0;

                return int.Parse(result.ToString());
            }
            catch(Exception e)
            {
                Logger.Log.Error($"UpdateDTimeStartMoobileSendings: {e.Message}");
                if(e.InnerException != null)
                {
                    Logger.Log.Error($"UpdateDTimeStartMoobileSendings: InnerException: {e.InnerException.Message}");
                }
                return 0;
            }
            finally
            {
                _model.Database.Connection.Close();
            }
        }

        private int InsertToMobileSendings(StartPostBindingModel start)
        {
            try
            {
                Guid g = Guid.NewGuid();

                _model.Database.Connection.Open();

                DbCommand command = _model.Database.Connection.CreateCommand();
                command.CommandText = $"insert into MobileSendings (IDCard, IDPost, DTimeStart, Guid)" +
                    $"values ((select IDCard from Cards where CardNum = '{start.card}'), " +
                    $"(select IDPost from Posts where QrCode = '{start.post}'), " +
                    $"'{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}', '{g.ToString().ToUpper()}'); " +
                    $"select scope_identity()";

                var result = command.ExecuteScalar();
                
                return int.Parse(result.ToString());
            }
            catch(Exception e)
            {
                Logger.Log.Error($"InsertToMobileSendings: {e.Message}");
                if (e.InnerException != null)
                {
                    Logger.Log.Error($"InsertToMobileSendings: InnerException: {e.InnerException.Message}");
                }
                return 0;
            }
            finally 
            { 
                _model.Database.Connection.Close();
            }
        }

        [HttpPost]
        [ActionName("stop_post")]
        public HttpResponseMessage StopPost([FromBody]StopPostAppModel model)
        {
            Logger.InitLogger();

            Logger.Log.Debug($"StopPost: остановка поста");
            try
            {
                if(!CryptHash.CheckHashCode(model.hash, model.time_send.ToString("yyyy-MM-dd HH:mm:ss")))
                {
                    Logger.Log.Error($"StopPost: Хэш не прошёл проверку ({model.hash}, {model.time_send.ToString("yyyy-MM-dd HH:mm:ss")})");
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
                }

                if (string.IsNullOrEmpty(model.card))
                {
                    Logger.Log.Error($"StopPost: номер карты пустой");
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }

                var card = _model.Cards.Where(c => c.CardNum == model.card).FirstOrDefault();
                if (card == null)
                {
                    Logger.Log.Error($"StopPost: не найдена карта {model.card}");
                    return Request.CreateResponse(HttpStatusCode.Forbidden);
                }

                string ip = GetPostIp(model.post);
                string deviceCode = _model.Posts.Where(p => p.QRCode == model.post).Select(p => p.Device.Code).FirstOrDefault();

                if (string.IsNullOrEmpty(ip))
                {
                    Logger.Log.Error($"StopPost: не найден ip поста {model.post}");
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                HttpResponse resp = Sender.SendPost("http://" + ip + "записа эндпоинт", JsonConvert.SerializeObject(new StopPostDeviceParameters
                {
                    CardNum = model.card,
                    DeviceCode = deviceCode
                }));

                if (resp.StatusCode == 0)
                {
                    Logger.Log.Error("StartPost: Не удалось подключиться" + Environment.NewLine);
                    return Request.CreateResponse((HttpStatusCode)424);
                }

                if (resp.StatusCode == (HttpStatusCode)423)
                {
                    Logger.Log.Error(String.Format("StartPost: Post {0} is busy", model.post) + Environment.NewLine);
                    return Request.CreateResponse((HttpStatusCode)423);
                }

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch(Exception e)
            {
                Logger.Log.Error($"StopPost: произошла ошибка: {e.Message}" + Environment.NewLine + e.StackTrace);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Конец мойки
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Ретранслирую коды ответа от сервера мобильного приложения</returns>
        /// <response code="200">Удачно</response>
        /// <response code="513">Ошибка при работе с базой данных</response>
        [HttpPost]
        [ActionName("stop_post-dev")]
        public HttpResponseMessage StopPostDev([FromBody] StopPostBindingModel model)
        {
            Logger.InitLogger();

            Logger.Log.Debug($"StopPost: отправка списания по карте {model.card}");

            try
            {
                var res = UpdateMobileSendings(model);
                Logger.Log.Debug($"Обновлены {res} записи MobileSendings");
            }
            catch (Exception e)
            {
                Logger.Log.Error("Ошибка при записи времени конца и суммы в журнал отправок приложения: " + e.Message);
                return Request.CreateResponse((HttpStatusCode)513, "Ошибка при записи в базу");
            }

            if (model.balance > 0)
                try
                {
                    if (_model.Database.Exists())
                    {
                        _model.Database.Connection.Open();
                        Logger.Log.Debug("Db connection: " + _model.Database.Connection.State.ToString());

                        DbCommand commandBalance = _model.Database.Connection.CreateCommand();
                        commandBalance.CommandText = $"select top 1 " +
                            $"isnull(o.Balance, 0) " +
                            $"from Cards c " +
                            $"left join Operations o on o.IDCard = c.IDCard " +
                            $"and o.DTime = (select MAX(DTime) from Operations where IDCard = c.IDCard) " +
                            $"where c.CardNum = '{model.card}' " +
                            $"order by o.IDOperation desc";

                        DbCommand command = _model.Database.Connection.CreateCommand();
                        DbTransaction tran = _model.Database.Connection.BeginTransaction();
                        command.Transaction = tran;

                        try
                        {
                            command.CommandText = $"UPDATE Cards SET Balance = ({commandBalance.CommandText}) - {model.balance} WHERE CardNum = '{model.card}'";
                            Logger.Log.Debug("StopPost: command is: " + command.CommandText);
                            command.ExecuteNonQuery();

                            command.CommandText = $"INSERT INTO Operations (IDDevice, IDOperationType, IDCard, DTime, Amount, Balance, LocalizedBy, LocalizedID) " +
                            $"VALUES ((select IDDevice from Device where Code = '{model.post}'), (select IDOperationType from OperationTypes where Code = 'decrease'), " +
                            $"(select min(IDCard) as IDCard from Cards where CardNum = '{model.card}'), '{model.time_send.ToString("yyyy-MM-dd HH:mm:ss")}', {model.balance}, ({commandBalance.CommandText}) - {model.balance}, " +
                            $"(select IDDevice from Device where Code = '{model.post}'), 0);";
                            Logger.Log.Debug("StopPost: command is: " + command.CommandText);
                            command.ExecuteNonQuery();

                            tran.Commit();

                            command.CommandText = "SELECT SCOPE_IDENTITY();";

                            var serverID = command.ExecuteScalar();
                            Logger.Log.Debug("StopPost: операция добавлена. id = " + serverID.ToString() + Environment.NewLine);
                            _model.Database.Connection.Close();

                            _model.Database.Connection.Close();
                        }
                        catch (Exception e)
                        {
                            if (_model.Database.Connection.State == System.Data.ConnectionState.Open)
                            {
                                tran.Rollback();
                                _model.Database.Connection.Close();
                            }

                            Logger.Log.Error("StopPost: ошибка записи в базу\n" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                            return Request.CreateResponse((HttpStatusCode)513, "Ошибка при записи в базу");
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.Log.Error("StopPostDev: ошибка при записи операции в базу.\n" + e.Message + Environment.NewLine + e.StackTrace);
                }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        private int UpdateMobileSendings(StopPostBindingModel stop)
        {
            _model.Database.Connection.Open();

            DbCommand command = _model.Database.Connection.CreateCommand();
            DbTransaction tran = _model.Database.Connection.BeginTransaction();
            command.Transaction = tran;
            try
            {
                command.CommandText = $"update MobileSendings " +
                    $"set DTimeEnd = '{stop.time_send:yyyy-MM-dd HH:mm:ss.fff}', amount = {stop.balance} " +
                    $"where IDMobileSending in " +
                    $"(select top 1 IDMobileSending " +
                    $"from MobileSendings ms " +
                    $"where IDCard = (select IDCard from Cards where CardNum = '{stop.card}') " +
                    $"and IDPost = (select p.IDpost from Posts p join Device d on d.IDDevice = p.IDDevice where d.Code = '{stop.post}') " +
                    $"and ms.DTimeEnd is null " +
                    $"order by ms.DTimeStart desc); ";

                var res = command.ExecuteNonQuery();
                if (res > 0)
                {
                    command.CommandText = $"update MobileSendings " +
                        $"set DTimeEnd = '{stop.time_send:yyyy-MM-dd HH:mm:ss.fff}', amount = 0 " +
                        $"where IDMobileSending in " +
                        $"(select IDMobileSending " +
                        $"from MobileSendings ms " +
                        $"where IDCard = (select IDCard from Cards where CardNum = '{stop.card}') " +
                        $"and IDPost = (select p.IDpost from Posts p join Device d on d.IDDevice = p.IDDevice where d.Code = '{stop.post}') " +
                        $"and ms.DTimeEnd is null) ";

                    //var result = command.ExecuteScalar();
                    res += command.ExecuteNonQuery();
                    tran.Commit();
                    
                }

                return res;
            }
            catch (Exception e)
            {
                Logger.Log.Error("UpdateMobileSendings: " + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                tran.Rollback();
                return 0;
            }
            finally
            {
                _model.Database.Connection.Close();
            }
        }

        private int WriteResponseToMibileSending(StopPostBindingModel stop, HttpResponse response)
        {
            _model.Database.Connection.Open();
            DbCommand command = _model.Database.Connection.CreateCommand();
            command.CommandText = $"update MobileSendings " +
                $"set StatusCode = {((int)response.StatusCode)}, ResultMessage = '{response.ResultMessage}' " +
                $"where IDCard = (select IDCard from Cards where CardNum = '{stop.card}') " +
                $"and IDPost = (select p.IDpost from Posts p join Device d on d.IDDevice = p.IDDevice where d.Code = '{stop.post}') " +
                $"and DTimeEnd = '{stop.time_send:yyyy-MM-dd HH:mm:ss.fff}' " +
                $"and Amount = {stop.balance}";

            var result = command.ExecuteNonQuery();
            return result;
        }

        private string PrepareWashCodeForMobile(string code)
        {
            string washCode = "";

            try
            {
                if (_model.Database.Exists())
                {
                    _model.Database.Connection.Open();

                    DbCommand commandWashCode = _model.Database.Connection.CreateCommand();
                    commandWashCode.CommandText = "select " +
                        "replace(w.Code, 'М', 'm')" +
                        "from " +
                        "Device d " +
                        "join Posts p on p.IDDevice = d.IDDevice " +
                        "join Wash w on w.IDWash = p.IDWash " +
                        "where 1 = 1 " +
                        $"and d.Code = '{code}'";
                    washCode = commandWashCode.ExecuteScalar().ToString();

                    _model.Database.Connection.Close();
                }
            }
            catch (Exception e)
            {
                if (_model.Database.Connection.State == System.Data.ConnectionState.Open)
                {
                    _model.Database.Connection.Close();
                }

                Logger.Log.Error("Ошибка при преведении кода поста к коду мойки для отправки мобильному приложению.\n" + e.Message + Environment.NewLine);
            }

            return washCode;
        }

        private string GetPostIp(string qrCode)
        {
            try
            {
                Device device = _model.Device.Find(_model.Posts.Where(p => p.QRCode.Equals(qrCode)).FirstOrDefault().IDDevice);

                return device.IpAddress;
            }
            catch (NullReferenceException e)
            {
                Logger.Log.Error("GetPostIp: не удалось найти ip поста");
                return "";
            }
        }

        /// <summary>
        /// Выпуск новой карты из мобльного приложения
        /// </summary>
        /// <param name="newCard"></param>
        /// <returns>Заголовок: CardNum - номер выпущенной карты (при удачной работе метода)</returns>
        /// <response code="200">Удачно</response>
        /// <response code="400">неверный формат входных данных</response>
        /// <response code="500">Внутренняя ошибка</response>
        /// <response code="409">У пользователя с таким номером уже есть карта</response>
        /// <response code="401">Хэш не прошёл проверку</response>
        /// <response code="204">Некорректные входные данные</response>
        /// <response code="513">Ошибка при работе с базой данных</response>
        [HttpPost]
        [ActionName("new_card")]
        public HttpResponseMessage NewCard([FromBody] NewCard newCard)
        {
            Logger.InitLogger();

            if (newCard != null)
            {
                try
                {
                    Logger.Log.Debug(String.Format("NewCard: Запуск с параметрами: номер телефона: {0}", newCard.phone));

                    if (/*CryptHash.CheckHashCode(newCard.hash, newCard.time_send)*/ true)
                    {
                        if(!newCard.phone.StartsWith("7") || newCard.phone.Length != 11)
                        {
                            Logger.Log.Error($"Incorrect phone format: {newCard.phone}");
                            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Неправильный формат номера");
                        }
                        if (_model.Database.Exists())
                        {
                            _model.Database.Connection.Open();
                            Logger.Log.Debug("Db connection: " + _model.Database.Connection.State.ToString());

                            List<Cards> cards = GetCardsByPhone(newCard.phone);
                            if (cards.Count > 0)
                            {
                                _model.Database.Connection.Close();
                                Logger.Log.Error($"NewCard: У пользователя есть карта: {cards.FirstOrDefault().CardNum}" + Environment.NewLine);
                                //return Request.CreateErrorResponse(HttpStatusCode.Conflict, new Exception(cards.FirstOrDefault().CardNum));
                                return Request.CreateResponse(HttpStatusCode.Conflict, cards.FirstOrDefault().CardNum);

                            }

                            DbCommand command = _model.Database.Connection.CreateCommand();
                            //command.CommandText = "select " +
                            //    "min(v.Num) " +
                            //    "from NumsMobileCards v " +
                            //    "left join Cards c on c.CardNum = v.Num " +
                            //    "where c.CardNum is null";

                            //var cardNum = command.ExecuteScalar();

                            PhoneFormatter formattedPhone = new PhoneFormatter(newCard.phone);

                            DbTransaction tran = _model.Database.Connection.BeginTransaction();
                            command.Transaction = tran;

                            try
                            {
                                command.CommandText = $"insert into Owners (Phone, PhoneInt, LocalizedBy, LocalizedID) values ('{formattedPhone.phone}', {formattedPhone.phoneInt}, 0, 0)";
                                command.ExecuteNonQuery();
                                command.CommandText = $"insert into Cards (IDOwner, CardNum,  IDCardStatus, IDCardType, LocalizedBy, LocalizedID) values (scope_identity(), '{formattedPhone.phoneInt}', 1, 4, 0, 0)";
                                command.ExecuteNonQuery();
                                command.CommandText = $"insert into Operations (IDDevice, IDOperationType, IDCard, DTime, Amount, Balance, LocalizedBy, LocalizedID) " +
                                       $"values ((select IDDevice from Device where Code = 'MOB-EM'), " +
                                       $"(select IDOperationType from OperationTypes ot where ot.Code = 'activation'), " +
                                       $"scope_identity(), '{newCard.time_send}', " +
                                       $"0, 0, (select IDDevice from Device where Code = 'MOB-EM'), 0);";
                                command.ExecuteNonQuery();

                                Logger.Log.Debug($"NewCard: добавлены Owner и Card. CardNum = {formattedPhone.phoneInt.ToString()}" + Environment.NewLine);
                                tran.Commit();
                            }
                            catch(SqlException e)
                            {
                                if(e.Number == 2627)
                                {
                                    Logger.Log.Error($"NewCard: unique key constraint, CardNum = {formattedPhone.phoneInt}\n" + e.Message + Environment.NewLine);
                                    tran.Rollback();
                                    _model.Database.Connection.Close();

                                    return Request.CreateResponse(HttpStatusCode.OK);
                                }
                                Logger.Log.Error("NewCard: ошибка транзакции.\n" + e.Message + Environment.NewLine);
                                tran.Rollback();
                                _model.Database.Connection.Close();

                                return Request.CreateResponse((HttpStatusCode)513, "Ошибка при записи в базу");
                            }
                            catch (Exception e)
                            {
                                Logger.Log.Error("NewCard: ошибка транзакции.\n" + e.Message + Environment.NewLine);
                                tran.Rollback();
                                _model.Database.Connection.Close();

                                return Request.CreateResponse((HttpStatusCode)513, "Ошибка транзакции");
                            }

                            var response = Request.CreateResponse();

                            response.StatusCode = HttpStatusCode.OK;
                            response.Headers.Add("CardNum", formattedPhone.phoneInt.ToString());

                            _model.Database.Connection.Close();

                            return response;
                        }
                        return Request.CreateResponse((HttpStatusCode)513, "База данных не найдена");
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

        private List<Cards> GetCardsByPhone(string phone)
        {
            PhoneFormatter formattedPhone = new PhoneFormatter(phone);

            List<Cards> cards = _model.Cards.Where(c => c.IDOwner == _model.Owners.Where(o => o.PhoneInt == formattedPhone.phoneInt).FirstOrDefault().IDOwner).ToList();

            return cards;
        }

        /// <summary>
        /// Отправка выпущенной на разменном аппарате карты с пополнением на сервер мобильного приложения  (хэш не проверяется)
        /// </summary>
        /// <param name="newCard"></param>
        /// <returns>Ретранслирую коды ответов от сервера мобильного приложения</returns>
        /// <response code="200">Удачно</response>
        /// <response code="409">У пользователя уже есть карта</response>
        /// <response code="500">Внутренняя ошибка</response>
        /// <response code="204">Некорректные входные значения</response>
        /// <response code="513">Ошибка при работе с базой данных</response>
        [HttpPost]
        [ActionName("send_new_card_dev")]
        public HttpResponseMessage SendNewCardDev([FromBody] NewCardDev newCard)
        {
            Logger.InitLogger();

            try
            {
                if (newCard != null)
                {
                    Logger.Log.Debug("SendNewCardDev: запуск с параметрами:\n" + JsonConvert.SerializeObject(newCard));

                    DateTime dtime = DateTime.Now;

                    NewCard card = new NewCard
                    {
                        card = newCard.card,
                        phone = newCard.phone,
                        time_send = dtime.ToString("yyyy-MM-dd HH:mm:ss"),
                        hash = CryptHash.GetHashCode(dtime.ToString("yyyy-MM-dd HH:mm:ss"))
                    };

                    PhoneFormatter formattedPhone = new PhoneFormatter(card.phone);

                    if (CardIsExists(newCard.card))
                    {
                        Logger.Log.Error("SendNewCardDev: карта с таким номером существует" + Environment.NewLine);
                        return Request.CreateResponse(HttpStatusCode.Conflict, "Card Exists");
                    }

                    // запись в нашу базу новую карту
                    if (_model.Database.Exists())
                    {
                        _model.Database.Connection.Open();
                        Logger.Log.Debug("Db connection: " + _model.Database.Connection.State.ToString());

                        List<Cards> cards = GetCardsByPhone(newCard.phone);
                        if (cards.Count > 0)
                        {
                            _model.Database.Connection.Close();
                            Logger.Log.Error("SendNewCardDev: У пользователя есть карта" + Environment.NewLine);
                            return Request.CreateResponse(HttpStatusCode.Conflict, "Owner Exists");
                        }

                        DbCommand command = _model.Database.Connection.CreateCommand();

                        DbTransaction tran = _model.Database.Connection.BeginTransaction();
                        command.Transaction = tran;

                        try
                        {
                            command.CommandText = $"insert into Owners (Phone, PhoneInt, LocalizedBy, LocalizedID) values ('{formattedPhone.phone}', {formattedPhone.phoneInt}, 1, 0)";
                            command.ExecuteNonQuery();
                            command.CommandText = $"insert into Cards (IDOwner, CardNum,  IDCardStatus, IDCardType, LocalizedBy, LocalizedID, Balance) " +
                                $"values (scope_identity(), '{newCard.card}', (select IDCardStatus from CardStatuses cs where cs.Code = 'norm'), " +
                                $"(select IDCardType from CardTypes ct where ct.Code = 'client'), (select IDChanger from Changers ch join Device d on d.IDDevice = ch.IDDevice where d.Code = '{newCard.changer}'), 0, " +
                                $"{newCard.value})";
                            command.ExecuteNonQuery();
                            command.CommandText = $"insert into Operations (IDDevice, IDOperationType, IDCard, DTime, Amount, Balance, LocalizedBy, LocalizedID) " +
                                $"values ((select IDDevice from Device where Code = '{newCard.changer}'), " +
                                $"(select IDOperationType from OperationTypes ot where ot.Code = 'activation'), " +
                                $"scope_identity(), '{newCard.time_send}', " +
                                $"0, 0, (select IDDevice from Device where Code = '{newCard.changer}'), 0);";
                            command.ExecuteNonQuery();

                            Logger.Log.Debug($"SendNewCardDev: добавлены Owner и Card. CardNum = {newCard.card} DTime = {newCard.time_send}" + Environment.NewLine);
                            tran.Commit();
                        }
                        catch (Exception e)
                        {
                            Logger.Log.Error("SendNewCardDev: ошибка транзакции.\n" + e.Message + Environment.NewLine);
                            tran.Rollback();
                            _model.Database.Connection.Close();

                            return Request.CreateResponse((HttpStatusCode)513, "Ошибка транзакции");
                        }
                    }
                    else
                    {
                        Logger.Log.Error("SendNewCardDev: базы данных не существует.\n" + Environment.NewLine);
                        _model.Database.Connection.Close();
                        return Request.CreateResponse((HttpStatusCode)513, "База данных не найдена");
                    }

                    // отправка карты в приложение
                    HttpResponse resp = Sender.SendPost("http://loyalty.myeco24.ru/api/externaldb/user-create", JsonConvert.SerializeObject(card));

                    Logger.Log.Debug("SendNewCardDev: отправлена карта: " + JsonConvert.SerializeObject(card));

                    if (resp.StatusCode != HttpStatusCode.OK)
                    {
                        Logger.Log.Debug("SendNewCardDev: отправлена новая карта. Ответ сервера: " + JsonConvert.SerializeObject(resp) + Environment.NewLine);
                        //return Request.CreateResponse(resp.StatusCode);
                    }

                    Logger.Log.Debug("SendNewCardDev: отправлена новая карта. Ответ сервера: " + JsonConvert.SerializeObject(resp));

                    // запись в нашу базу внесения
                    if (_model.Database.Exists())
                    {
                        DbCommand commandBalance = _model.Database.Connection.CreateCommand();
                        commandBalance.CommandText = $"select top 1 " +
                            $"isnull(o.Balance, 0) " +
                            $"from Cards c " +
                            $"left join Operations o on o.IDCard = c.IDCard " +
                            $"and o.DTime = (select MAX(DTime) from Operations where IDCard = c.IDCard) " +
                            $"where c.CardNum = '{newCard.card}' " +
                            $"order by o.IDOperation desc";

                        DbCommand command = _model.Database.Connection.CreateCommand();
                        DbTransaction tran = _model.Database.Connection.BeginTransaction();
                        command.Transaction = tran;

                        try
                        {
                            command.CommandText = $"UPDATE Cards SET Balance = ({commandBalance.CommandText}) + {newCard.value} WHERE CardNum = '{newCard.card}'";
                            Logger.Log.Debug("SendNewCardDev: command is: " + command.CommandText);
                            command.ExecuteNonQuery();

                            command.CommandText = $"INSERT INTO Operations (IDDevice, IDOperationType, IDCard, DTime, Amount, Balance, LocalizedBy, LocalizedID) " +
                            $"VALUES ((select IDDevice from Device where Code = '{newCard.changer}'), (select IDOperationType from OperationTypes where Code = 'increase'), " +
                            $"(select min(IDCard) as IDCard from Cards where CardNum = '{newCard.card}'), '{newCard.time_send}', {newCard.value}, {newCard.value}, " +
                            $"(select IDDevice from Device where Code = '{newCard.changer}'), 0);";
                            Logger.Log.Debug("SendNewCardDev: command is: " + command.CommandText);
                            command.ExecuteNonQuery();

                            tran.Commit();

                            command.CommandText = "SELECT SCOPE_IDENTITY();";

                            var serverID = command.ExecuteScalar();
                            Logger.Log.Debug("SendNewCardDev: операция добавлена. id = " + serverID.ToString() + Environment.NewLine);
                            _model.Database.Connection.Close();

                            _model.Database.Connection.Close();
                        }
                        catch (Exception e)
                        {
                            if (_model.Database.Connection.State == System.Data.ConnectionState.Open)
                            {
                                tran.Rollback();
                                _model.Database.Connection.Close();
                            }

                            Logger.Log.Error("SendNewCardDev: ошибка записи в базу\n" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                            return Request.CreateResponse((HttpStatusCode)513, "Ошибка при записи в базу");
                        }
                    }
                    else
                    {
                        Logger.Log.Error("SendNewCardDev: базы данных не существует.\n" + Environment.NewLine);
                        _model.Database.Connection.Close();
                        return Request.CreateResponse((HttpStatusCode)513, "База данных не найдена");
                    }

                    int washID = _model.Changers.Where(c => c.IDDevice == _model.Device.Where(d => d.Code.Equals(newCard.changer)).FirstOrDefault().IDDevice).FirstOrDefault().IDWash;
                    string washCode = _model.Wash.Find(washID).Code;

                    // отправка пополнения на пост
                    resp = Sender.SendPost("http://loyalty.myeco24.ru/api/externaldb/set-replenish", JsonConvert.SerializeObject(new Increase
                    {
                        time_send = dtime.ToString("yyyy-MM-dd HH:mm:ss"),
                        hash = CryptHash.GetHashCode(dtime.ToString("yyyy-MM-dd HH:mm:ss")),
                        card = newCard.card,
                        value = newCard.value,
                        wash_id = washCode,
                        operation_time = dtime.ToString("yyyy-MM-dd HH:mm:ss")
                    }));

                    Logger.Log.Debug("SendNewCardDev: отправлено пополнение. Ответ сервера:" + JsonConvert.SerializeObject(resp) + Environment.NewLine);

                    _model.Database.Connection.Close();
                    return Request.CreateResponse(HttpStatusCode.OK);
                }

                return Request.CreateResponse(HttpStatusCode.NoContent);
            }
            catch (Exception e)
            {
                Logger.Log.Error("SendNewCardDev: произошла ошибка:\n" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Получить список карт по номеру телефона
        /// </summary>
        /// <param name="id">Номер телефона</param>
        /// <returns>Список номеров карт</returns>
        /// <response code="200">Ок</response>
        /// <response code="513">Ошибка при работе с базой данных</response>
        [HttpGet]
        [ActionName("get_cards")]
        public HttpResponseMessage GetCardsList(string id)
        {
            Logger.InitLogger();
            try
            {
                Logger.Log.Debug("GetCardsList: запуск с параметром " + id);

                List<Cards> cards = GetCardsByPhone(id);

                List<CardsNumType> result = new List<CardsNumType>();

                foreach (Cards c in cards)
                {
                    CardsNumType cnt = new CardsNumType();
                    cnt.cardNum = c.CardNum;

                    string localizedBy = "";
                    if (_model.Device.Find(c.LocalizedBy) == null)
                        localizedBy = "MOB-EM";
                    else
                        localizedBy = _model.Device.Find(c.LocalizedBy).Code;

                    if (localizedBy.Equals("MOB-EM"))
                        cnt.type = "virtual";
                    else
                        cnt.type = "real";

                    cnt.balance = c.Balance;

                    result.Add(cnt);
                }

                Logger.Log.Debug("GetCardsList: список карт:\n" + JsonConvert.SerializeObject(result));

                return Request.CreateResponse<List<CardsNumType>>(HttpStatusCode.OK, result);
            }
            catch (Exception e)
            {
                Logger.Log.Error("GetCardsList: " + e.Message.ToString() + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Получить номер телефона владельца карты
        /// </summary>
        /// <param name="id">Номер карты</param>
        /// <returns>Номер телефона владельца</returns>
        /// <response code="200">Ок</response>
        /// <response code="404">Не найден номер</response>
        /// <response code="500">Внуренняя ошибка сервера</response>
        /// <response code="513">Ошибка при работе с базой данных</response>
        [HttpGet]
        [ActionName("get_phone")]
        public HttpResponseMessage GetPhone(string id)
        {
            Logger.InitLogger();
            try
            {
                Logger.Log.Debug("GetPhone: запуск с параметром " + id);

                Cards card = _model.Cards.Where(c => c.CardNum.Equals(id)).FirstOrDefault();

                if (card == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                string phone = _model.Owners.Find(card.IDOwner).Phone;

                GetPhoneResult result = new GetPhoneResult();
                result.phone = phone;
                result.balance = card.Balance;

                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception e)
            {
                Logger.Log.Error("GetPhone: " + e.Message.ToString() + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Получить номера технических карт
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Ок</response>
        /// <response code="513">Ошибка при работе с базой данных</response>
        [HttpGet]
        [ActionName("tech_cards")]
        public HttpResponseMessage GetTechCards()
        {
            Logger.InitLogger();
            try
            {
                TechCards result = new TechCards();

                List<Cards> cards = _model.Cards.Where(c => !c.IDCardType.Equals(_model.CardTypes.Where(ct => ct.Code.Equals("client")).FirstOrDefault().IDCardType)).ToList();

                result.cleanUp = new List<string>();
                result.collect = new List<string>();
                result.doors = new List<string>();
                result.service = new List<string>();

                int idService = _model.CardTypes.Where(ct => ct.Code.Equals("service")).FirstOrDefault().IDCardType;
                int idClean = _model.CardTypes.Where(ct => ct.Code.Equals("clean")).FirstOrDefault().IDCardType;
                int idDoors = _model.CardTypes.Where(ct => ct.Code.Equals("doors")).FirstOrDefault().IDCardType;
                int idCollect = _model.CardTypes.Where(ct => ct.Code.Equals("collect")).FirstOrDefault().IDCardType;

                foreach (Cards c in cards)
                {
                    if (c.IDCardType == idClean)
                        result.cleanUp.Add(c.CardNum);
                    if (c.IDCardType == idCollect)
                        result.collect.Add(c.CardNum);
                    if (c.IDCardType == idDoors)
                        result.doors.Add(c.CardNum);
                    if (c.IDCardType == idService)
                        result.service.Add(c.CardNum);
                }

                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception e)
            {
                Logger.Log.Error("GetTechCards: " + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        private bool CardIsExists(string cardNum)
        {
            return _model.Cards.Where(c => c.CardNum.Equals(cardNum)).FirstOrDefault() != null;
        }

        /// <summary>
        /// Изменение номера карты
        /// </summary>
        /// <param name="change">Страый и новый номер</param>
        /// <returns></returns>
        /// <response code="200">Ок</response>
        /// <response code="404">Не найдена карта</response>
        /// <response code="409">Новый номер уже есть в базе</response>
        /// <response code="500">Внутренняя ошибка</response>
        /// <response code="513">Ошибка при работе с базой данных</response>
        [HttpPut]
        [ActionName("change_card")]
        public HttpResponseMessage ChangeCard([FromBody] CardChange change)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            Logger.InitLogger();

            try
            {
                Logger.Log.Debug("ChangeCard: запуск с параметрами: " + JsonConvert.SerializeObject(change));

                if (!CardIsExists(change.oldNum))
                {
                    Logger.Log.Error("ChangeCard: не найдена карта" + Environment.NewLine);
                    return Request.CreateResponse(HttpStatusCode.NotFound, "С таким номером карта не найдена");
                }

                if (CardIsExists(change.newNum))
                {
                    Logger.Log.Error("ChangeCard: новый номер карты уже записан в базе" + Environment.NewLine);
                    return Request.CreateResponse(HttpStatusCode.Conflict, "Уже существует карта с такм номером");
                }

                if (!_model.Database.Exists())
                {
                    Logger.Log.Error("ChangeCard: база данных не найдена" + Environment.NewLine);
                    return Request.CreateResponse((HttpStatusCode)513, "База данных не найдена");
                }

                _model.Database.Connection.Open();
                Logger.Log.Debug("Db connection: " + _model.Database.Connection.State.ToString());

                DbCommand command = _model.Database.Connection.CreateCommand();
                command.CommandText = $"UPDATE Cards SET CardNum = '{change.newNum}', LocalizedBy = (select IDDevice from Device where Code = '{change.changer}') " +
                    $"WHERE CardNum = '{change.oldNum}'";
                Logger.Log.Debug("ChangeCard: command is: " + command.CommandText);

                var id = command.ExecuteScalar();
                Logger.Log.Debug("ChangeCard: номер карты изменён" + Environment.NewLine);

                _model.Database.Connection.Close();

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                if (_model.Database.Connection.State == System.Data.ConnectionState.Open)
                    _model.Database.Connection.Close();
                Logger.Log.Error("ChangeCard: " + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Внутренняя ошибка сервера");
            }
        }

        /// <summary>
        /// Проверка на существование карты
        /// </summary>
        /// <param name="cardNum">Номер карты</param>
        /// <returns></returns>
        /// <response code="200">Есть такая карта</response>
        /// <response code="404">Нет такой карты</response>
        /// <response code="400">Некорректные входные данные</response>
        /// <response code="500">Внутренняя ошибка</response>
        /// <response code="513">Ошибка при работе с базой данных</response>
        [HttpGet]
        [ActionName("card_exists")]
        public HttpResponseMessage IsCardExists(string cardNum)
        {
            try
            {
                if (cardNum != null && cardNum != "")
                {
                    if (CardIsExists(cardNum))
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, true);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, false);
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                return Request.CreateResponse(HttpStatusCode.OK, true);
            }
            catch (Exception e)
            {
                if (_model.Database.Connection.State == System.Data.ConnectionState.Open)
                    _model.Database.Connection.Close();
                Logger.Log.Error("ChangeCard: " + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Внутренняя ошибка сервера");
            }
        }

        /// <summary>
        /// Получить списания по карте за определённый день
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Ретранслирую коды ответа от сервера мобильного приложения</returns>
        /// <response code="200">В теле ответа сумма списаний</response>
        /// <response code="204">Входной объект null</response>
        /// <response code="400">Не указан номер карты</response>
        /// <response code="401">Хэш не прошёл проверку</response>
        /// <response code="404">Карта не найдена</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        /// <response code="513">Ошибка при работе с базой данных</response>
        [HttpPost]
        [ActionName("get_decrease")]
        public HttpResponseMessage GetDecrease([FromBody]GetDecreaseModel model)
        {
            Logger.InitLogger();

            try
            {
                if (model == null)
                {
                    Logger.Log.Error("GetDecrease: Входной параметр == null" + Environment.NewLine);
                    return Request.CreateResponse(HttpStatusCode.NoContent);
                }

                if (model.card == null || model.card == "")
                {
                    Logger.Log.Error("GetDecrease: Не указан номер карты" + Environment.NewLine);
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }

                if (CryptHash.CheckHashCode(model.hash, model.time_send))
                {
                    Logger.Log.Error("GetDecrease: Хэш не прошёл проверку" + Environment.NewLine);
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
                }

                if (!CardIsExists(model.card))
                {
                    Logger.Log.Error($"Карта {model.card} не найдена" + Environment.NewLine);
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                if (model.date == null)
                    model.date = DateTime.Today.AddDays(-1).Date;

                if (!_model.Database.Exists())
                {
                    Logger.Log.Error("GetDecrease: база данных не найдена!" + Environment.NewLine);
                    return Request.CreateResponse((HttpStatusCode)513, "База данных не найдена");
                }

                _model.Database.Connection.Open();
                DbCommand command = _model.Database.Connection.CreateCommand();
                command.CommandText = $"select SUM(coalesce(o.Amount, 0)) " +
                    $"from Operations o " +
                    $"join OperationTypes ot on ot.IDOperationType = o.IDOperationType " +
                    $"join Cards c on c.IDCard = o.IDCard " +
                    $"where ot.Code = 'decrease' " +
                    $"and c.CardNum = '{model.card}' " +
                    $"and o.DTime < '{model.date.Date:yyyy-MM-dd} 23:59:59' " +
                    $"and o.DTime > '{model.date.Date:yyyy-MM-dd} 00:00:00'";

                var decrease = command.ExecuteScalar();

                int.TryParse(decrease.ToString(), out int result);

                if (decrease == null)
                {
                    Logger.Log.Info($"GetDecrease: запрос вернул null. Входные данные:" + JsonConvert.SerializeObject(model) + Environment.NewLine);
                    return Request.CreateResponse(HttpStatusCode.OK, 0);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, result);
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error("GetDecrease: " + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }
    }
}
