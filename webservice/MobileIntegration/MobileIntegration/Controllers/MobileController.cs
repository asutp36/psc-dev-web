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

        /// <summary>
        /// Пополнить баланс карты (хэш не проверяется)
        /// </summary>
        /// <param name="increase">Данные поплнения (хэн не обязательно)</param>
        /// <returns></returns>
        /// <response code="200">Баланс пополнен</response>
        /// <response code="500">Произошла ошибка</response>
        /// <response code="204">Некорректное входное значение</response>
        [HttpPost]
        [ActionName("increase_dev")]
        public HttpResponseMessage IncreaseBalanceDev([FromBody]IncreaseFromChanger increase)
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
                            $"(select IDCard from Cards where CardNum = '{increase.card}'), '{increase.time_send.ToString("yyyy-MM-dd HH:mm:ss")}', {increase.value}, ({commandBalance.CommandText}) + {increase.value}, " +
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
                            return Request.CreateResponse(HttpStatusCode.InternalServerError, "Ошибка при записи в базу");
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
        [HttpPost]
        [ActionName("increase")]
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
                                $"(select IDCard from Cards where CardNum = '{increase.card}'), '{increase.time_send.ToString("yyyy-MM-dd HH:mm:ss")}', {increase.value}, ({commandBalance.CommandText}) + {increase.value}, " +
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
                                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Ошибка при записи в базу");
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
        [HttpPost]
        [ActionName("get_balance")]
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
        public HttpResponseMessage GetBalanceDev([FromBody]GetBalance getBalance)
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

                                Logger.Log.Debug("Результат: " + resp.StatusCode);
                                return Request.CreateResponse(resp.StatusCode);
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
                    Logger.Log.Error("StartPost: " + e.Message.ToString() + Environment.NewLine + e.StackTrace.ToString() + Environment.NewLine);
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
                                int amount = model.balance;

                                string ip = GetPostIp(model.post);

                                if(ip == null || ip.Equals(""))
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

        /// <summary>
        /// Конец мойки
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Ретранслирую коды ответа от сервера мобильного приложения</returns>
        /// <response code="200">Удачно</response>
        [HttpPost]
        [ActionName("stop_post-dev")]
        public HttpResponseMessage StopPostDev([FromBody]StartPostBindingModel model)
        {
            Logger.InitLogger();
            
            Logger.Log.Debug($"StopPost: отправка списания по карте {model.card}");

            string washCode = "";

            if (model.balance > 0)
                try
                {
                    if (_model.Database.Exists())
                    {
                        _model.Database.Connection.Open();
                        Logger.Log.Debug("Db connection: " + _model.Database.Connection.State.ToString());

                        DbCommand commandWashCode = _model.Database.Connection.CreateCommand();
                        commandWashCode.CommandText = "select " +
                            "replace(w.Code, 'М', 'm')" +
                            "from " +
                            "Device d " +
                            "join Posts p on p.IDDevice = d.IDDevice " +
                            "join Wash w on w.IDWash = p.IDWash " +
                            "where 1 = 1 " +
                            $"and d.Code = '{model.post}'";
                        washCode = commandWashCode.ExecuteScalar().ToString();

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
                            $"(select IDCard from Cards where CardNum = '{model.card}'), '{model.time_send.ToString("yyyy-MM-dd HH:mm:ss")}', {model.balance}, ({commandBalance.CommandText}) - {model.balance}, " +
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
                            return Request.CreateResponse(HttpStatusCode.InternalServerError, "Ошибка при записи в базу");
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.Log.Error("StopPostDev: ошибка при записи операции в базу.\n" + e.Message + Environment.NewLine + e.StackTrace);
                }

            string qrCode = "";
            try 
            {

                //qrCode = _model.Posts.Where(p => p.IDDevice == _model.Device.Where(d => d.Code.Equals(model.post)).FirstOrDefault().IDDevice).FirstOrDefault().QRCode;
            }
            catch(Exception e)
            {
                Logger.Log.Error("StopPostDev: не найден пост.\n" + e.Message + Environment.NewLine);
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Post not found");
            }

            Logger.Log.Debug("StopPostDev: отправка конца мойки");

            HttpResponse resp = Sender.SendPost("http://188.225.79.69/api/externaldb/set-waste", JsonConvert.SerializeObject(new Decrease(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), model.card, model.time_send.ToString("yyyy-MM-dd HH:mm:ss"), /*model.post*/ washCode, model.balance)));

            Logger.Log.Debug("StopPostDev: Ответ от их сервера: " + resp.ResultMessage);

            return Request.CreateErrorResponse(resp.StatusCode, resp.ResultMessage);
        }

        private string GetPostIp(string qrCode)
        {
            try
            {
                Device device = _model.Device.Find(_model.Posts.Where(p => p.QRCode.Equals(qrCode)).FirstOrDefault().IDDevice);

                return device.IpAddress;
            }
            catch(NullReferenceException e)
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
        /// <response code="500">Внутренняя ошибка</response>
        /// <response code="409">У пользователя с таким номером уже есть карта</response>
        /// <response code="401">Хэш не прошёл проверку</response>
        /// <response code="204">Некорректные входные данные</response>
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

                    if (CryptHash.CheckHashCode(newCard.hash, newCard.time_send))
                    {
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
                            catch (Exception e)
                            {
                                Logger.Log.Error("NewCard: ошибка транзакции.\n" + e.Message + Environment.NewLine);
                                tran.Rollback();
                                _model.Database.Connection.Close();

                                return Request.CreateResponse(HttpStatusCode.InternalServerError);
                            }

                            var response = Request.CreateResponse();

                            response.StatusCode = HttpStatusCode.OK;
                            response.Headers.Add("CardNum", formattedPhone.phoneInt.ToString());

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
        [HttpPost]
        [ActionName("send_new_card_dev")]
        public HttpResponseMessage SendNewCardDev([FromBody]NewCardDev newCard)
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

                            return Request.CreateResponse(HttpStatusCode.InternalServerError);
                        }
                    }
                    else
                    {
                        Logger.Log.Error("SendNewCardDev: базы данных не существует.\n" + Environment.NewLine);
                        _model.Database.Connection.Close();
                        return Request.CreateResponse(HttpStatusCode.InternalServerError);
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
                            $"(select IDCard from Cards where CardNum = '{newCard.card}'), '{newCard.time_send}', {newCard.value}, {newCard.value}, " +
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
                            return Request.CreateResponse(HttpStatusCode.InternalServerError, "Ошибка при записи в базу");
                        }
                    }
                    else
                    {
                        Logger.Log.Error("SendNewCardDev: базы данных не существует.\n" + Environment.NewLine);
                        _model.Database.Connection.Close();
                        return Request.CreateResponse(HttpStatusCode.InternalServerError);
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

                foreach(Cards c in cards)
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
            catch(Exception e)
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
        [HttpPut]
        [ActionName("change_card")]
        public HttpResponseMessage ChangeCard([FromBody]CardChange change)
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
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "Внутренняя ошибка сервера");
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
            catch(Exception e)
            {
                if (_model.Database.Connection.State == System.Data.ConnectionState.Open)
                    _model.Database.Connection.Close();
                Logger.Log.Error("ChangeCard: " + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Внутренняя ошибка сервера");
            }
        }
    }
}
