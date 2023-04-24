using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SynchronizationService.Models;
using SynchronizationService.Controllers.Supplies;
using System.Data.Common;
using Newtonsoft.Json;
using System.Globalization;
using System.Data.SqlClient;

namespace SynchronizationService.Controllers
{
    public class SyncController : ApiController
    {
        private ModelDb _model = new ModelDb();

        /// <summary>
        /// Синхронизация записей таблицы Operations
        /// </summary>
        /// <param name="operation">Данные операции</param>
        /// <returns>ServerID в заголовках при удачной записи</returns>
        /// <response code="200">ОК, ServerID в заголовке</response>
        /// <response code="204">Входные данные = null</response>
        /// <response code="500">Внутренняя ошибка, читать тело ответа</response>
        [HttpPost]
        [ActionName("op")]
        public HttpResponseMessage PostOp([FromBody]Operations operation)
        {
            Logger.InitLogger();
            if (operation != null)
            {
                try
                {
                    if (_model.Database.Exists())
                    {
                        _model.Database.Connection.Open();
                        Logger.Log.Debug("Db connection: " + _model.Database.Connection.State.ToString());

                        DbCommand command = _model.Database.Connection.CreateCommand();

                        command.CommandText = "INSERT INTO Operations (IDCard, IDChanger, IDOperationType, DTime, Amount, Balance, LocalizedBy, LocalizedID)" +
                                                $" VALUES((select IDCard from Cards where LocalizedBy =  {operation.LocalizedBy} and LocalizedID = {operation.IDCard}), " +
                                                $"(select IDChanger from Changers ch join Device d on d.IDDevice = ch.IDDevice where d.Code = '{operation.LocalizedBy}'), {operation.IDOperationType}, " +
                                                $"\'{operation.DTime.ToString("yyyyMMdd HH:mm:ss")}\', {operation.Amount}, {operation.Balance}, {operation.LocalizedBy}, {operation.IDOperation});" +
                                                " SELECT SCOPE_IDENTITY()";

                        Logger.Log.Debug("Command is: " + command.CommandText);

                        var id = command.ExecuteScalar();
                        Int32 serverID = Convert.ToInt32(id.ToString());
                        Logger.Log.Debug("Operation added serverID:" + serverID);

                        _model.Database.Connection.Close();

                        var responseGood = Request.CreateResponse(HttpStatusCode.OK);
                        responseGood.Headers.Add("ServerID", serverID.ToString());
                        return responseGood;
                    }

                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "База данных не найдена");
                }

                catch (Exception e)
                {
                    Logger.Log.Error(e.Message.ToString());
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                }
            }

            var responseBad = Request.CreateResponse(HttpStatusCode.NoContent);

            return responseBad;
        }

        /// <summary>
        /// Синхронизация записей таблицы Owners
        /// </summary>
        /// <param name="owner">Данные владельца</param>
        /// <returns>ServerID в заголовках при удачной записи</returns>
        /// <response code="200">ОК, ServerID в заголовке</response>
        /// <response code="204">Входные данные = null</response>
        /// <response code="500">Внутренняя ошибка, читать тело ответа</response>
        [HttpPost]
        [ActionName("owner")]
        public HttpResponseMessage PostOw([FromBody]Owners owner)
        {
            Logger.InitLogger();
            if (owner != null)
            {
                try
                {
                    if (_model.Database.Exists())
                    {
                        _model.Database.Connection.Open();
                        Logger.Log.Debug("Db connection: " + _model.Database.Connection.State.ToString());

                        DbCommand command = _model.Database.Connection.CreateCommand();
                        command.CommandText = "INSERT INTO Owners (Phone, LocalizedBy, LocalizedID)" +
                                                $" VALUES('{owner.Phone}', {owner.LocalizedBy}, {owner.IDOwner});" +
                                                " SELECT SCOPE_IDENTITY()";

                        Logger.Log.Debug("Command is: " + command.CommandText);

                        var id = command.ExecuteScalar();
                        Int32 serverID = Convert.ToInt32(id.ToString());
                        Logger.Log.Debug("Owner added serverID:" + serverID);

                        _model.Database.Connection.Close();

                        var responseGood = Request.CreateResponse(HttpStatusCode.OK);
                        responseGood.Headers.Add("ServerID", serverID.ToString());
                        return responseGood;
                    }

                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "База данных не найдена");
                }

                catch (Exception e)
                {
                    Logger.Log.Error(e.Message.ToString());
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                }
            }

            var responseBad = Request.CreateResponse(HttpStatusCode.NoContent);

            return responseBad;

        }

        /// <summary>
        /// Синхронизация таблицы Cards
        /// </summary>
        /// <param name="card">Данные карты</param>
        /// <returns>ServerID в заголовках при удачной записи</returns>
        /// <response code="200">ОК, ServerID в заголовке</response>
        /// <response code="204">Входные данные = null</response>
        /// <response code="500">Внутренняя ошибка, читать тело ответа</response>
        [HttpPost]
        [ActionName("card")]
        public HttpResponseMessage PostCard([FromBody]Cards card)
        {
            Logger.InitLogger();
            if (card != null)
            {
                try
                {
                    if (_model.Database.Exists())
                    {
                        _model.Database.Connection.Open();
                        Logger.Log.Debug("Db connection: " + _model.Database.Connection.State.ToString());

                        DbCommand command = _model.Database.Connection.CreateCommand();
                        command.CommandText = "INSERT INTO Cards (IDOwner, CardNum, IDCardStatus, IDCardType, LocalizedBy, LocalizedID)" +
                                                $" VALUES((select IDOwner from Owners where LocalizedBy =  {card.LocalizedBy} and LocalizedID = {card.IDOwner}), " +
                                                $" {card.CardNum}, {card.IDCardStatus}, {card.IDCardType}, {card.LocalizedBy}, {card.IDCard});" +
                                                " SELECT SCOPE_IDENTITY()";

                        Logger.Log.Debug("Command is: " + command.CommandText);

                        var id = command.ExecuteScalar();
                        Int32 serverID = Convert.ToInt32(id.ToString());
                        Logger.Log.Debug("Card added serverID:" + serverID);

                        _model.Database.Connection.Close();

                        var responseGood = Request.CreateResponse(HttpStatusCode.OK);
                        responseGood.Headers.Add("ServerID", serverID.ToString());
                        return responseGood;
                    }

                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "База данных не найдена");
                }

                catch (Exception e)
                {
                    Logger.Log.Error(e.Message.ToString());
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                }
            }

            var responseBad = Request.CreateResponse(HttpStatusCode.NoContent);

            return responseBad;
        }

        /// <summary>
        /// Снхронизация таблицы Finance
        /// </summary>
        /// <param name="finance"></param>
        /// <returns>ServerID в заголовках при удачной записи</returns>
        /// <response code="200">ОК, ServerID в заголовке</response>
        /// <response code="204">Входные данные = null</response>
        /// <response code="500">Внутренняя ошибка, читать тело ответа</response>
        [HttpPost]
        [ActionName("finance")]
        public HttpResponseMessage PostFinance([FromBody]FinanceFromRequest finance)
        {
            Logger.InitLogger();
            if (finance != null)
            {
                try
                {
                    if (_model.Database.Exists())
                    {
                        _model.Database.Connection.Open();
                        Logger.Log.Debug("Db connection: " + _model.Database.Connection.State.ToString());

                        DbCommand command = _model.Database.Connection.CreateCommand();
                        command.CommandText = "INSERT INTO Finance (IDDevice, IDFinanceType, DTime, Amount)" +
                                                $" VALUES((select IDDevice from Device where ServerID = {finance.DeviceServerID}), " +
                                                $" (select IDFinanceType from FinanceType where ServerID = {finance.FinanceTypeServerID}), " +
                                                $"\'{finance.DTime.ToString("yyyyMMdd HH:mm:ss")}\'," +
                                                $" {finance.Amount}); SELECT SCOPE_IDENTITY()";

                        Logger.Log.Debug("Command is: " + command.CommandText);

                        var id = command.ExecuteScalar();
                        Int32 serverID = Convert.ToInt32(id.ToString());
                        Logger.Log.Debug("Finance added serverID:" + serverID);

                        _model.Database.Connection.Close();

                        var responseGood = Request.CreateResponse(HttpStatusCode.OK);
                        responseGood.Headers.Add("ServerID", serverID.ToString());
                        return responseGood;
                    }

                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "База данных не найдена");
                }

                catch (Exception e)
                {
                    Logger.Log.Error(e.Message.ToString());
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                }
            }

            var responseBad = Request.CreateResponse(HttpStatusCode.NoContent);

            return responseBad;
        }

        /// <summary>
        /// Снхронизация таблицы EventIncrease без id сессии
        /// </summary>
        /// <param name="increase">Данные внесения</param>
        /// <returns>ServerID в заголовках при удачной записи</returns>
        /// <response code="200">ОК, ServerID в заголовке</response>
        /// <response code="204">Входные данные = null</response>
        /// <response code="500">Внутренняя ошибка, читать тело ответа</response>
        /// <response code="409">Есть операция с таким же временем</response>
        [HttpPost]
        [ActionName("eincrease")]
        public HttpResponseMessage PostEventIncrease([FromBody]EIncreaseFromRequest increase)
        {
            Logger.InitLogger();
            
            try
            {           
                if (increase != null)
                {
                    Logger.Log.Debug("PostEventIncrease: Запуск с параметрами:\n" + JsonConvert.SerializeObject(increase));

                    if (_model.Database.Exists())
                    {
                        _model.Database.Connection.Open();
                        Logger.Log.Debug("PostEventIncrease: Соединение с БД: " + _model.Database.Connection.State);

                        DbCommand command = _model.Database.Connection.CreateCommand();
                        command.CommandText = "BEGIN TRANSACTION; " +
                            "INSERT INTO Event (IDPost, IDEventKind, DTime, IDEventPost) " +
                            $"VALUES ((select p.IDPost from Posts p where p.IDDevice = (select d.IDDevice from Device d where d.Code = \'{increase.Device}\')), " +
                            $"(select ek.IDEventKind from EventKind ek where ek.Code = \'{increase.Kind}\'), \'{increase.DTime.ToString("yyyyMMdd HH:mm:ss.fff")}\', {increase.IDEventPost}); " +
                            "INSERT INTO EventIncrease (IDEvent, amount, m10, b10, b50, b100, b200, balance) " +
                            $"VALUES ((SELECT SCOPE_IDENTITY()), {increase.Amount}, {increase.m10}, {increase.b10}, {increase.b50}, {increase.b100},{increase.b200}, " +
                            $"{increase.Balance}); " +
                            "SELECT IDENT_CURRENT(\'Event\')" +
                            "COMMIT;";

                        Logger.Log.Debug("Command is: " + command.CommandText);

                        var id = command.ExecuteScalar();
                        _model.Database.Connection.Close();

                        Int32 serverID = Convert.ToInt32(id.ToString());

                        Logger.Log.Debug("PostEventIncrease: Event добавлен. IDEvent: " + serverID.ToString() + Environment.NewLine);

                        var response = Request.CreateResponse(HttpStatusCode.OK);
                        response.Headers.Add("ServerID", serverID.ToString());

                        if(increase.Kind == "cardincrease" && increase.CardNum != null)
                        {
                            int updated = UpdateMobileSendings(increase);
                            Logger.Log.Debug($"PostEventIncrease: обновлены в MobileSendings {updated} записей");
                        }
                        return response;
                    }
                    else
                    {
                        Logger.Log.Error("PostEventIncrease: База данных не найдена!" + Environment.NewLine);
                        return Request.CreateResponse(HttpStatusCode.InternalServerError);
                    }
                }
                else
                {
                    Logger.Log.Error("PostEventIncrease: increase == null. Ошибка в данных запроса." + Environment.NewLine);
                    return Request.CreateResponse(HttpStatusCode.NoContent);
                }
            }
            catch (SqlException e)
            {
                if(e.Number == 2627)
                {
                    Logger.Log.Error("PostEventIncrease: " + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                    return Request.CreateResponse(HttpStatusCode.Conflict);
                }
                Logger.Log.Error("PostEventIncrease: " + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            catch (Exception ex)
            {
                Logger.Log.Error("PostEventIncrease: " + ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            finally
            {
                if (_model.Database.Connection.State != System.Data.ConnectionState.Closed)
                    _model.Database.Connection.Close();
            }
        }

        /// <summary>
        /// Снхронизация таблицы EventIncrease с записью id сессии
        /// </summary>
        /// <param name="increase">Данные внесения</param>
        /// <returns>ServerID в заголовках при удачной записи</returns>
        /// <response code="200">ОК, ServerID в заголовке</response>
        /// <response code="204">Входные данные = null</response>
        /// <response code="500">Внутренняя ошибка, читать тело ответа</response>
        /// <response code="409">Есть операция с таким же временем</response>
        /// <response code="400">Не найдена сессия для этого внесения</response>
        [HttpPost]
        [ActionName("eincrease-w-session")]
        public HttpResponseMessage PostEventIncreaseWithSession([FromBody] EIncreaseFromRequest increase)
        {
            Logger.InitLogger();

            try
            {
                if (increase != null)
                {
                    Logger.Log.Debug("PostEventIncrease: Запуск с параметрами:\n" + JsonConvert.SerializeObject(increase));

                    if (!CheckSessionExists(increase.IDPostSession, increase.Device))
                    {
                        Logger.Log.Debug($"PostEventIncrease: не найдена сессия id={increase.IDPostSession} на посте {increase.Device}");
                        return Request.CreateResponse(HttpStatusCode.BadRequest);
                    }

                    if (_model.Database.Exists())
                    {
                        _model.Database.Connection.Open();
                        Logger.Log.Debug("PostEventIncrease: Соединение с БД: " + _model.Database.Connection.State);

                        DbCommand command = _model.Database.Connection.CreateCommand();
                        
                        command.CommandText = "BEGIN TRANSACTION; " +
                            "INSERT INTO Event (IDPost, IDEventKind, DTime, IDEventPost) " +
                            $"VALUES ((select p.IDPost from Posts p where p.IDDevice = (select d.IDDevice from Device d where d.Code = \'{increase.Device}\')), " +
                            $"(select ek.IDEventKind from EventKind ek where ek.Code = \'{increase.Kind}\'), \'{increase.DTime.ToString("yyyyMMdd HH:mm:ss.fff")}\', {increase.IDEventPost}); " +
                            "INSERT INTO EventIncrease (IDEvent, amount, m10, b10, b50, b100, b200, balance, IDPostSession) " +
                            $"VALUES ((SELECT SCOPE_IDENTITY()), {increase.Amount}, {increase.m10}, {increase.b10}, {increase.b50}, {increase.b100},{increase.b200}, " +
                            $@"{increase.Balance}, (select ps.IDPostSession
                                        from PostSession ps
                                        where ps.IDSessionOnPost = {increase.IDPostSession}
                                        and ps.IDPost = (select p.IDPost from Posts p where p.IDDevice = (select d.IDDevice from Device d where d.Code = '{increase.Device}')))); " +
                            "SELECT IDENT_CURRENT(\'Event\')" +
                            "COMMIT;";
                        Logger.Log.Debug("Command is: " + command.CommandText);

                        var id = command.ExecuteScalar();
                        _model.Database.Connection.Close();

                        Int32 serverID = Convert.ToInt32(id.ToString());

                        Logger.Log.Debug("PostEventIncrease: Event добавлен. IDEvent: " + serverID.ToString() + Environment.NewLine);

                        var response = Request.CreateResponse(HttpStatusCode.OK);
                        response.Headers.Add("ServerID", serverID.ToString());

                        if (increase.Kind == "cardincrease" && increase.CardNum != null)
                        {
                            int updated = UpdateMobileSendings(increase);
                            Logger.Log.Debug($"PostEventIncrease: обновлены в MobileSendings {updated} записей");
                        }

                        return response;
                    }
                    else
                    {
                        Logger.Log.Error("PostEventIncrease: База данных не найдена!" + Environment.NewLine);
                        return Request.CreateResponse(HttpStatusCode.InternalServerError);
                    }
                }
                else
                {
                    Logger.Log.Error("PostEventIncrease: increase == null. Ошибка в данных запроса." + Environment.NewLine);
                    return Request.CreateResponse(HttpStatusCode.NoContent);
                }
            }
            catch (SqlException e)
            {
                if (e.Number == 2627)
                {
                    Logger.Log.Error("PostEventIncrease: " + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                    return Request.CreateResponse(HttpStatusCode.Conflict);
                }
                Logger.Log.Error("PostEventIncrease: " + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            catch (Exception ex)
            {
                Logger.Log.Error("PostEventIncrease: " + ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            finally
            {
                if (_model.Database.Connection.State != System.Data.ConnectionState.Closed)
                    _model.Database.Connection.Close();
            }
        }

        private int UpdateMobileSendings(EIncreaseFromRequest increase)
        {
            string sqlDetails;
            if (string.IsNullOrEmpty(increase.Details))
            {
                sqlDetails = "details = NULL";
            }
            else
            {
                sqlDetails = $"details = '{increase.Details}'";
            }
            _model.Database.Connection.Open();

            DbCommand command = _model.Database.Connection.CreateCommand();
            DbTransaction tran = _model.Database.Connection.BeginTransaction();
            command.Transaction = tran;
            try
            {
                command.CommandText = $"update MobileSendings " +
                    $"set DTimeEnd = '{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}', amount = {increase.Amount}, {sqlDetails} " +
                    $"where IDMobileSending in " +
                    $"(select top 1 IDMobileSending " +
                    $"from MobileSendings ms " +
                    $"where IDCard = (select IDCard from Cards where CardNum = '{increase.CardNum}') " +
                    $"and IDPost = (select p.IDpost from Posts p join Device d on d.IDDevice = p.IDDevice where d.Code = '{increase.Device}') " +
                    $"and ms.DTimeEnd is null " +
                    $"and ms.DTimeStart <= '{increase.DTime.AddSeconds(2):yyyy-MM-dd HH:mm:ss.fff}'" +
                    $"order by ms.DTimeStart desc); ";

                var res = command.ExecuteNonQuery();
                if (res > 0)
                {
                    command.CommandText = $"update MobileSendings " +
                        $"set DTimeEnd = '{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}', amount = 0 " +
                        $"where IDMobileSending in " +
                        $"(select IDMobileSending " +
                        $"from MobileSendings ms " +
                        $"where IDCard = (select IDCard from Cards where CardNum = '{increase.CardNum}') " +
                        $"and IDPost = (select p.IDpost from Posts p join Device d on d.IDDevice = p.IDDevice where d.Code = '{increase.Device}') " +
                        $"and ms.DTimeStart <= '{increase.DTime.AddSeconds(2):yyyy-MM-dd HH:mm:ss.fff}'" +
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

        private bool CheckSessionExists(int idSession, string postCode)
        {
            Logger.InitLogger();

            if (_model.Database.Exists()) 
            {
                _model.Database.Connection.Open();

                DbCommand command = _model.Database.Connection.CreateCommand();
                command.CommandText = $@"select ps.IDPostSession
                                        from PostSession ps
                                        where ps.IDSessionOnPost = {idSession}
                                        and ps.IDPost = (select p.IDPost from Posts p where p.IDDevice = (select d.IDDevice from Device d where d.Code = '{postCode}'))";
                var id = command.ExecuteScalar();
                _model.Database.Connection.Close();

                return id != null;
            }
            else
            {
                Logger.Log.Error("CheckSessionExists: база данных не найдена");
            }

            return false;
        }

        /// <summary>
        /// Синхронизация таблицы EventMode
        /// </summary>
        /// <param name="mode">Данные режима</param>
        /// <returns>ServerID в заголовках при удачной записи</returns>
        /// <response code="200">ОК, ServerID в заголовке</response>
        /// <response code="204">Входные данные = null</response>
        /// <response code="500">Внутренняя ошибка, читать тело ответа</response>
        /// <response code="409">Есть операция с таким же временем</response>
        [HttpPost]
        [ActionName("emode")]
        public HttpResponseMessage PostEventMode([FromBody]EModeFromRequest mode)
        {
            Logger.InitLogger();

            try
            {
                if(mode != null)
                {
                    Logger.Log.Debug("PostEventMode: Запуск с параметрами:\n" + JsonConvert.SerializeObject(mode));

                    if (_model.Database.Exists())
                    {   
                        string finish = DateTime.Compare(mode.DTimeFinish, DateTime.Parse("2000-01-01 00:00:00")) <= 0 ? 
                            "null" :
                            '\'' + mode.DTimeFinish.ToString("yyyyMMdd HH:mm:ss.fff") + '\''; 

                        _model.Database.Connection.Open();
                        Logger.Log.Debug("PostEventMode: Соединение с БД: " + _model.Database.Connection.State);
                        
                        DbCommand command = _model.Database.Connection.CreateCommand();
                        command.CommandText = "BEGIN TRANSACTION; " +
                            "INSERT INTO Event (IDPost, IDEventKind, DTime, IDEventPost) " +
                            $"VALUES ((select p.IDPost from Posts p where p.IDDevice = (select d.IDDevice from Device d where d.Code = \'{mode.Device}\')), " +
                            $"(select ek.IDEventKind from EventKind ek where ek.Code = \'mode\'), \'{mode.DTimeStart.ToString("yyyyMMdd HH:mm:ss.fff")}\', {mode.IDEventPost}); " +
                            "INSERT INTO EventMode (IDEvent, IDMode, DTimeStart, DTimeFinish, Duration, PaymentSign, Cost, CardTypeCode, CardNum, Discount) " +
                            $"VALUES ((SELECT SCOPE_IDENTITY()), (select m.IDMode from Mode m where m.Code = \'{mode.Mode}\'), \'{mode.DTimeStart.ToString("yyyyMMdd HH:mm:ss.fff")}\', {finish}, " +
                            $"{mode.Duration}, {mode.PaymentSign}, {mode.Cost.ToString().Replace(",", CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator)}, \'{mode.CardTypeCode}\', \'{mode.CardNum}\', {mode.Discount}); " +
                            "SELECT IDENT_CURRENT(\'Event\')" +
                            "COMMIT;";

                        Logger.Log.Debug("Command is: " + command.CommandText);

                        var id = command.ExecuteScalar();
                        _model.Database.Connection.Close();

                        Int32 serverID = Convert.ToInt32(id.ToString());

                        Logger.Log.Debug("PostEventMode: Event добавлен. IDEvent: " + serverID.ToString() + Environment.NewLine);

                        var response = Request.CreateResponse(HttpStatusCode.OK);
                        response.Headers.Add("ServerID", serverID.ToString());
                        return response;
                    }
                    else
                    {
                        Logger.Log.Error("PostEventMode: База данных не найдена!" + Environment.NewLine);
                        return Request.CreateResponse(HttpStatusCode.InternalServerError);
                    }

                }
                else
                {
                    Logger.Log.Error("PostEventMode: mode == null. Ошибка в данных запроса." + Environment.NewLine);
                    return Request.CreateResponse(HttpStatusCode.NoContent);
                }
            }
            catch (SqlException e)
            {
                if (e.Number == 2627)
                {
                    Logger.Log.Error("PostEventIncrease: " + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                    return Request.CreateResponse(HttpStatusCode.Conflict);
                }
                Logger.Log.Error("PostEventIncrease: " + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            catch (Exception ex)
            {
                Logger.Log.Error("PostEventMode: " + ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            finally
            {
                if (_model.Database.Connection.State != System.Data.ConnectionState.Closed)
                    _model.Database.Connection.Close();
            }
        }

        /// <summary>
        /// Синхронзация таблицы EventCollect
        /// </summary>
        /// <param name="collect">Данные инкассации</param>
        /// <returns>ServerID в заголовках при удачной записи</returns>
        /// <response code="200">ОК, ServerID в заголовке</response>
        /// <response code="204">Входные данные = null</response>
        /// <response code="500">Внутренняя ошибка, читать тело ответа</response>
        /// <response code="409">Есть операция с таким же временем</response>
        [HttpPost]
        [ActionName("ecollect")]
        public HttpResponseMessage PostEventCollect([FromBody]ECollectFromRequest collect)
        {
            Logger.InitLogger();

            try
            {
                if (collect != null)
                {
                    Logger.Log.Debug("PostEventCollect: Запуск с параметрами:\n" + JsonConvert.SerializeObject(collect));

                    if (_model.Database.Exists())
                    {
                        _model.Database.Connection.Open();
                        Logger.Log.Debug("PostEventCollect: Соединение с БД: " + _model.Database.Connection.State);

                        DbCommand command = _model.Database.Connection.CreateCommand();
                        command.CommandText = "BEGIN TRANSACTION; " +
                            "INSERT INTO Event (IDPost, IDEventKind, DTime, IDEventPost) " +
                            $"VALUES ((select p.IDPost from Posts p where p.IDDevice = (select d.IDDevice from Device d where d.Code = \'{collect.Device}\')), " +
                            $"(select ek.IDEventKind from EventKind ek where ek.Code = \'collect\'), \'{collect.DTime.ToString("yyyyMMdd HH:mm:ss.fff")}\', {collect.IDEventPost}); " +
                            "INSERT INTO EventCollect (IDEvent, amount, m10, b10, b50, b100, b200) " +
                            $"VALUES ((SELECT SCOPE_IDENTITY()), {collect.Amount}, {collect.m10}, {collect.b10}, {collect.b50}, {collect.b100},{collect.b200}); " +
                            "SELECT IDENT_CURRENT(\'Event\')" +
                            "COMMIT;";

                        //Logger.Log.Debug("Command is: " + command.CommandText);

                        var id = command.ExecuteScalar();
                        _model.Database.Connection.Close();

                        Int32 serverID = Convert.ToInt32(id.ToString());

                        Logger.Log.Debug("PostEventCollect: Event добавлен. IDEvent: " + serverID.ToString() + Environment.NewLine);

                        var response = Request.CreateResponse(HttpStatusCode.OK);
                        response.Headers.Add("ServerID", serverID.ToString());
                        return response;
                    }
                    else
                    {
                        Logger.Log.Error("PostEventCollect: База данных не найдена!" + Environment.NewLine);
                        return Request.CreateResponse(HttpStatusCode.InternalServerError);
                    }
                }
                else
                {
                    Logger.Log.Error("PostEventCollect: increase == null. Ошибка в данных запроса." + Environment.NewLine);
                    return Request.CreateResponse(HttpStatusCode.NoContent);
                }
            }
            catch (SqlException e)
            {
                if (e.Number == 2627)
                {
                    Logger.Log.Error("PostEventIncrease: " + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                    return Request.CreateResponse(HttpStatusCode.Conflict);
                }
                Logger.Log.Error("PostEventIncrease: " + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            catch (Exception ex)
            {
                Logger.Log.Error("PostEventCollect: " + ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            finally
            {
                if (_model.Database.Connection.State != System.Data.ConnectionState.Closed)
                    _model.Database.Connection.Close();
            }
        }

        /// <summary>
        /// Синхронзация таблицы PostSession
        /// </summary>
        /// <param name="session">Данные сессии</param>
        /// <returns>ServerID в заголовках при удачной записи</returns>
        /// <response code="200">ОК, ServerID в заголовке</response>
        /// <response code="204">Входные данные = null</response>
        /// <response code="500">Внутренняя ошибка, читать тело ответа</response>  
        [HttpPost]
        [ActionName("psession")]
        public HttpResponseMessage PostSession([FromBody]PostSessionData session)
        {
            Logger.InitLogger();

            try
            {
                if (session != null)
                {
                    Logger.Log.Debug("PostSession: Запуск с параметрами:\n" + JsonConvert.SerializeObject(session));

                    if (_model.Database.Exists())
                    {
                        _model.Database.Connection.Open();
                        Logger.Log.Debug("PostSession: Соединение с БД: " + _model.Database.Connection.State);

                        DbCommand command = _model.Database.Connection.CreateCommand();
                        command.CommandText = "BEGIN TRANSACTION; " +
                            "INSERT INTO PostSession (IDPost, IDSessionOnPost, StartDTime, QR, FiscalError, StopDTime, AmountCash, AmountBank) " +
                            $"VALUES ((select p.IDPost from Posts p where p.IDDevice = (select d.IDDevice from Device d where d.Code = \'{session.postCode}\')), " +
                            $@"{session.idSessionOnPost}, '{session.startDTime.ToString("yyyyMMdd HH:mm:ss.fff")}', '{session.qr}', '{session.fiscalError}', " +
                            $@"'{session.stopDTime.ToString("yyyyMMdd HH:mm: ss.fff")}', {session.amountCash}, {session.amountBank}); " +
                            "SELECT IDENT_CURRENT(\'PostSession\')" +
                            "COMMIT;";

                        Logger.Log.Debug("Command is: " + command.CommandText);

                        var id = command.ExecuteScalar();
                        _model.Database.Connection.Close();

                        Int32 serverID = Convert.ToInt32(id.ToString());

                        Logger.Log.Debug("PostSession: Sesiont добавлена. IDSesion: " + serverID.ToString() + Environment.NewLine);

                        var response = Request.CreateResponse(HttpStatusCode.OK);
                        response.Headers.Add("ServerID", serverID.ToString());
                        return response;
                    }
                    else
                    {
                        Logger.Log.Error("PostSession: База данных не найдена!" + Environment.NewLine);
                        return Request.CreateResponse(HttpStatusCode.InternalServerError);
                    }
                }
                else
                {
                    Logger.Log.Error("PostSession: session == null. Ошибка в данных запроса." + Environment.NewLine);
                    return Request.CreateResponse(HttpStatusCode.NoContent);
                }

            }
            catch (SqlException e)
            {
                // это исключение, если на уровне бд будет ограничение на уникальные время+пост
                if (e.Number == 2627)
                {
                    Logger.Log.Error("PostSession: " + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                    return Request.CreateResponse(HttpStatusCode.Conflict);
                }
                Logger.Log.Error("PostSession: " + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            catch (Exception ex)
            {
                Logger.Log.Error("PostSession: " + ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            finally
            {
                if (_model.Database.Connection.State != System.Data.ConnectionState.Closed)
                    _model.Database.Connection.Close();
            }
        }

        /// <summary>
        /// Снхронизация клиентской сессии робота
        /// </summary>
        /// <param name="data">Данные клиентской сессии</param>
        /// <returns>ServerID в заголовках при удачной записи</returns>
        /// <response code="200">ОК, ServerID в заголовке</response>
        /// <response code="204">Входные данные = null</response>
        /// <response code="500">Внутренняя ошибка, читать тело ответа</response>
        /// <response code="409">Есть операция с таким же временем</response>
        [HttpPost]
        [ActionName("rsession")]
        public HttpResponseMessage RobotSession([FromBody] DataRobotSession data)
        {
            Logger.InitLogger();
            //Logger.Log.Debug("RobotEventSession: Запуск с параметрами:\n" + JsonConvert.SerializeObject(data));

            try
            {
                if (data != null)
                {
                    if (_model.Database.Exists())
                    {
                        _model.Database.Connection.Open();
                        //Logger.Log.Debug("RobotEventSession: Соединение с БД: " + _model.Database.Connection.State);

                        DbCommand command = _model.Database.Connection.CreateCommand();
                        command.CommandText = $@"
                            BEGIN TRANSACTION;
                            INSERT INTO RobotSession(IDPost, DTime, IDRobotProgram, IDSessionPost)
                            select
                            p.IDPost
                            , '{data.DTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}'
                            , (select IDRobotProgram from RobotProgram p where p.Code = '{data.ProgramCode}')
                            , {data.IDSessionPost}
                            from
                            Device d
                            join Posts p on p.IDDevice = d.IDDevice
                            where d.Code = '{data.DeviceCode}';
                            SELECT IDENT_CURRENT('RobotSession');
                            COMMIT;
                        ";

                        //Logger.Log.Debug("Command is: " + command.CommandText);

                        var id = command.ExecuteScalar();
                        _model.Database.Connection.Close();

                        Int32 serverID = Convert.ToInt32(id.ToString());

                        Logger.Log.Debug("RobotSession добавлен, inserted id=" + serverID.ToString() + Environment.NewLine);

                        var response = Request.CreateResponse(HttpStatusCode.OK);
                        response.Headers.Add("ServerID", serverID.ToString());
                        return response;
                    }
                    else
                    {
                        Logger.Log.Error("RobotSession: База данных не найдена!" + Environment.NewLine);
                        return Request.CreateResponse(HttpStatusCode.InternalServerError);
                    }
                }
                else
                {
                    Logger.Log.Error("RobotSession: data == null. Нет данных запроса." + Environment.NewLine);
                    return Request.CreateResponse(HttpStatusCode.NoContent);
                }
            }
            catch (SqlException e)
            {
                if (e.Number == 2627)
                {
                    Logger.Log.Error("RobotSession: " + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                    return Request.CreateResponse(HttpStatusCode.Conflict);
                }
                else
                {
                    Logger.Log.Error("RobotSession: " + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error("RobotSession: " + ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            finally
            {
                if (_model.Database.Connection.State != System.Data.ConnectionState.Closed)
                    _model.Database.Connection.Close();
            }
        }

        /// <summary>
        /// Снхронизация внесений на роботе
        /// </summary>
        /// <param name="data">Данные внесений</param>
        /// <returns>ServerID в заголовках при удачной записи</returns>
        /// <response code="200">ОК, ServerID в заголовке</response>
        /// <response code="204">Входные данные = null</response>
        /// <response code="500">Внутренняя ошибка, читать тело ответа</response>
        /// <response code="406">Не найдены зависимые объекты</response>
        /// <response code="409">Есть операция с таким же временем</response>
        [HttpPost]
        [ActionName("rincrease")]
        public HttpResponseMessage RobotEventIncrease([FromBody] DataRobotIncrease data)
        {
            Logger.InitLogger();
            //Logger.Log.Debug("RobotEventIncrease: Запуск с параметрами:\n" + JsonConvert.SerializeObject(data));

            bool transaction_success = false;

            if (data != null)
            {
                _model.Database.Connection.Open();
                using (var transaction = _model.Database.Connection.BeginTransaction())
                {
                    try
                    {
                        DbCommand command = _model.Database.Connection.CreateCommand();
                        command.Transaction = transaction;

                        command.CommandText = $@"select IDPost from Posts p join Device d on d.IDDevice = p.IDDevice where d.Code = '{data.DeviceCode}'";
                        //Logger.Log.Debug("Command is: " + command.CommandText);
                        var id = command.ExecuteScalar();
                        if (id == DBNull.Value)
                        {
                            string err_message = "Не найден IDPost по DeviceCode " + data.DeviceCode;
                            Logger.Log.Debug("RobotEventIncrease: " + err_message + Environment.NewLine);
                            return Request.CreateResponse(HttpStatusCode.NotFound, err_message);
                        }
                        Int32 IDPost = Convert.ToInt32(id.ToString());
                        //Logger.Log.Debug("RobotEventIncrease: IDPost=" + IDPost.ToString() + Environment.NewLine);

                        command.CommandText = $@"select IDRobotSession from RobotSession where IDpost = {IDPost} and IDSessionPost = {data.IDSessionPost}";
                        //Logger.Log.Debug("Command is: " + command.CommandText);
                        id = command.ExecuteScalar();
                        if (id == DBNull.Value)
                        {
                            Logger.Log.Debug("RobotEventIncrease: Не найден IDRobotSession по IDSessionPost " + data.IDSessionPost + Environment.NewLine);
                            return Request.CreateResponse(HttpStatusCode.NotAcceptable);
                        }
                        Int32 IDRobotSession = Convert.ToInt32(id.ToString());
                        //Logger.Log.Debug("RobotEventIncrease: IDSession=" + IDRobotSession.ToString() + Environment.NewLine);

                        command.CommandText = $@"select IDEventKind from EventKind where Code = '{data.EventKindCode}'";
                        //Logger.Log.Debug("Command is: " + command.CommandText);
                        id = command.ExecuteScalar();
                        if (id == DBNull.Value)
                        {
                            Logger.Log.Debug("RobotEventIncrease: Не найден IDEventKind по EventKindCode " + data.EventKindCode + Environment.NewLine);
                            return Request.CreateResponse(HttpStatusCode.NotFound);
                        }
                        Int32 IDEventKind = Convert.ToInt32(id.ToString());
                        //Logger.Log.Debug("RobotEventIncrease: IDEventKind=" + IDEventKind.ToString() + Environment.NewLine);

                        // Сохрнить Event
                        command.CommandText = $@"
                            insert into RobotEvent(IDRobotSession, IDPost, IDEventKind, DTime, IDEventPost)
                            values({IDRobotSession}, {IDPost}, {IDEventKind}, '{data.DTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}', {data.IDEventPost});
                            SELECT IDENT_CURRENT('RobotEvent');
                        ";
                        //Logger.Log.Debug("Command is: " + command.CommandText);
                        id = command.ExecuteScalar();
                        if (id == DBNull.Value)
                        {
                            Logger.Log.Error("RobotEventIncrease: Данные RobotEvent не добавлены" + Environment.NewLine);
                            return Request.CreateResponse(HttpStatusCode.ServiceUnavailable);
                        }
                        Int32 serverID = Convert.ToInt32(id.ToString());
                        Logger.Log.Debug("RobotEventIncrease добавлен, inserted id=" + serverID.ToString() + Environment.NewLine);

                        // Сохрнить EventIncrease
                        command.CommandText = $@"
                            insert into RobotEventIncrease(IDRobotEvent, amount, m10, b50, b100, b200, b500, b1000, b2000)
                            values({serverID}, {data.amount}, {data.m10}, {data.b50}, {data.b100}, {data.b200}, {data.b500}, {data.b1000}, {data.b2000})
                        ";
                        //Logger.Log.Debug("Command is: " + command.CommandText);
                        if (command.ExecuteNonQuery() < 1)
                        {
                            Logger.Log.Error("RobotEventIncrease: Данные RobotEventIncrease не добавлены" + Environment.NewLine);
                            return Request.CreateResponse(HttpStatusCode.ServiceUnavailable);
                        }


                        transaction_success = true;

                        var response = Request.CreateResponse(HttpStatusCode.OK);
                        response.Headers.Add("ServerID", serverID.ToString());
                        return response;
                    }
                    catch (SqlException e)
                    {
                        if (e.Number == 2627)
                        {
                            Logger.Log.Error("RobotEventIncrease: " + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                            return Request.CreateResponse(HttpStatusCode.Conflict);
                        }
                        else
                        {
                            Logger.Log.Error("RobotEventIncrease: " + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                            return Request.CreateResponse(HttpStatusCode.InternalServerError);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log.Error("RobotEventIncrease: " + ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine);
                        return Request.CreateResponse(HttpStatusCode.InternalServerError);
                    }
                    finally
                    {
                        if (transaction_success)
                            transaction.Commit();
                        else
                            transaction.Rollback();

                        if (_model.Database.Connection.State != System.Data.ConnectionState.Closed)
                        {
                            //Logger.Log.Error("RobotEventIncrease: Connection.Close" + Environment.NewLine);
                            _model.Database.Connection.Close();
                        }
                    }
                }
            }
            else
            {
                Logger.Log.Error("RobotEventIncrease: data == null. Нет данных запроса." + Environment.NewLine);
                return Request.CreateResponse(HttpStatusCode.NoContent);
            }
        }

        /// <summary>
        /// Снхронизация выдачи сдачи на роботе
        /// </summary>
        /// <param name="data">Данные сдачи</param>
        /// <returns>ServerID в заголовках при удачной записи</returns>
        /// <response code="200">ОК, ServerID в заголовке</response>
        /// <response code="204">Входные данные = null</response>
        /// <response code="500">Внутренняя ошибка, читать тело ответа</response>
        /// <response code="406">Не найдены зависимые объекты</response>
        /// <response code="409">Есть операция с таким же временем</response>
        [HttpPost]
        [ActionName("rpayout")]
        public HttpResponseMessage RobotEventPayout([FromBody] DataRobotPayout data)
        {
            Logger.InitLogger();
            Logger.Log.Debug("RobotEventPayout: Запуск с параметрами:\n" + JsonConvert.SerializeObject(data));

            bool transaction_success = false;

            if (data != null)
            {
                _model.Database.Connection.Open();
                using (var transaction = _model.Database.Connection.BeginTransaction())
                {
                    try
                    {
                        DbCommand command = _model.Database.Connection.CreateCommand();
                        command.Transaction = transaction;

                        command.CommandText = $@"select IDPost from Posts p join Device d on d.IDDevice = p.IDDevice where d.Code = '{data.DeviceCode}'";
                        //Logger.Log.Debug("Command is: " + command.CommandText);
                        var id = command.ExecuteScalar();
                        if (id == DBNull.Value)
                        {
                            Logger.Log.Debug("RobotEventPayout: Не найден IDPost по DeviceCode " + data.DeviceCode + Environment.NewLine);
                            return Request.CreateResponse(HttpStatusCode.NotFound);
                        }
                        Int32 IDPost = Convert.ToInt32(id.ToString());
                        //Logger.Log.Debug("RobotEventPayout: IDPost=" + IDPost.ToString() + Environment.NewLine);

                        command.CommandText = $@"select IDRobotSession from RobotSession where IDpost = {IDPost} and IDSessionPost = {data.IDSessionPost}";
                        //Logger.Log.Debug("Command is: " + command.CommandText);
                        id = command.ExecuteScalar();
                        if (id == DBNull.Value)
                        {
                            Logger.Log.Debug("RobotEventPayout: Не найден IDRobotSession по IDSessionPost " + data.IDSessionPost + Environment.NewLine);
                            return Request.CreateResponse(HttpStatusCode.NotAcceptable);
                        }
                        Int32 IDRobotSession = Convert.ToInt32(id.ToString());
                        //Logger.Log.Debug("RobotEventPayout: IDSession=" + IDRobotSession.ToString() + Environment.NewLine);

                        command.CommandText = $@"select IDEventKind from EventKind where Code = '{data.EventKindCode}'";
                        //Logger.Log.Debug("Command is: " + command.CommandText);
                        id = command.ExecuteScalar();
                        if (id == DBNull.Value)
                        {
                            Logger.Log.Debug("RobotEventPayout: Не найден IDEventKind по EventKindCode " + data.EventKindCode + Environment.NewLine);
                            return Request.CreateResponse(HttpStatusCode.NotFound);
                        }
                        Int32 IDEventKind = Convert.ToInt32(id.ToString());
                        //Logger.Log.Debug("RobotEventPayout: IDEventKind=" + IDEventKind.ToString() + Environment.NewLine);

                        // Сохранить Event
                        command.CommandText = $@"
                            insert into RobotEvent(IDRobotSession, IDPost, IDEventKind, DTime, IDEventPost)
                            values({IDRobotSession}, {IDPost}, {IDEventKind}, '{data.DTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}', {data.IDEventPost});
                            SELECT IDENT_CURRENT('RobotEvent');
                        ";
                        //Logger.Log.Debug("Command is: " + command.CommandText);
                        id = command.ExecuteScalar();
                        if (id == DBNull.Value)
                        {
                            Logger.Log.Error("RobotEventPayout: Данные RobotEvent не добавлены" + Environment.NewLine);
                            return Request.CreateResponse(HttpStatusCode.ServiceUnavailable);
                        }
                        Int32 serverID = Convert.ToInt32(id.ToString());
                        Logger.Log.Debug("RobotEventPayout добавлен, inserted id=" + serverID.ToString() + Environment.NewLine);

                        // Сохранить EventPayout
                        command.CommandText = $@"
                            insert into RobotEventPayout(IDRobotEvent, amount, b50, b100, storage_b50, storage_b100)
                            values({serverID}, {data.amount}, {data.b50}, {data.b100}, {data.storage_b50}, {data.storage_b100})
                        ";
                        //Logger.Log.Debug("Command is: " + command.CommandText);
                        if (command.ExecuteNonQuery() < 1)
                        {
                            Logger.Log.Error("RobotEventPayout: Данные RobotEventPayout не добавлены" + Environment.NewLine);
                            return Request.CreateResponse(HttpStatusCode.ServiceUnavailable);
                        }


                        transaction_success = true;

                        var response = Request.CreateResponse(HttpStatusCode.OK);
                        response.Headers.Add("ServerID", serverID.ToString());
                        return response;
                    }
                    catch (SqlException e)
                    {
                        if (e.Number == 2627)
                        {
                            Logger.Log.Error("RobotEventPayout: " + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                            return Request.CreateResponse(HttpStatusCode.Conflict);
                        }
                        else
                        {
                            Logger.Log.Error("RobotEventPayout: " + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                            return Request.CreateResponse(HttpStatusCode.InternalServerError);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log.Error("RobotEventPayout: " + ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine);
                        return Request.CreateResponse(HttpStatusCode.InternalServerError);
                    }
                    finally
                    {
                        if (transaction_success)
                            transaction.Commit();
                        else
                            transaction.Rollback();

                        if (_model.Database.Connection.State != System.Data.ConnectionState.Closed)
                        {
                            //Logger.Log.Error("RobotEventPayout: Connection.Close" + Environment.NewLine);
                            _model.Database.Connection.Close();
                        }
                    }
                }
            }
            else
            {
                Logger.Log.Error("RobotEventPayout: data == null. Нет данных запроса." + Environment.NewLine);
                return Request.CreateResponse(HttpStatusCode.NoContent);
            }
        }
    }
}
