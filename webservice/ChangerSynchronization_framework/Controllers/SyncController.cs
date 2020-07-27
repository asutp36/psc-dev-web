using ChangerSynchronization_framework.Controllers.Supplies;
using ChangerSynchronization_framework.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ChangerSynchronization_framework.Controllers
{
    public class SyncController : ApiController
    {
        private ModelDb _model = new ModelDb();

        /// <summary>
        /// Синхронизация события на разменнике
        /// </summary>
        /// <param name="model">Полная информация о событии</param>
        /// <returns></returns>
        /// <response code="201">Событие записано</response>
        /// <response code="503">Не получилось добавить EventChanger</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        /// <response code="400">Модель не прошла валидацию</response>
        [HttpPost]
        [ActionName("event")]
        public HttpResponseMessage PostEventChanger([FromBody]EventChangerFull model)
        {
            Logger.InitLogger();

            try
            {
                Logger.Log.Debug("PostEventChanger: запуск с параметрами" + JsonConvert.SerializeObject(model));

                if (!ModelState.IsValid)
                {
                    Logger.Log.Error("PostEventChanger: модель не прошла валидацию." + Environment.NewLine);
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }

                if (!_model.Database.Exists())
                {
                    Logger.Log.Error("PostEventChanger: база данных не найдена" + Environment.NewLine);
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                }

                InsertFullEventChangerResult insertResult = WriteEventChanger(model);

                if (insertResult.serverId == null)
                {
                    Logger.Log.Error("PostEventChanger: не удалось записать в базу EventChanger." + Environment.NewLine);
                    return Request.CreateResponse((HttpStatusCode)503, "Неудалось записть в базу");
                }

                return Request.CreateResponse(HttpStatusCode.Created, JsonConvert.SerializeObject(insertResult));
            }
            catch (Exception e)
            {
                Logger.Log.Error("PostEventChanger: " + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e.Message);
            }
        }

        private InsertFullEventChangerResult WriteEventChanger(EventChangerFull eventFull)
        {
            Logger.InitLogger();
            try
            {
                _model.Database.Connection.Open();
                Logger.Log.Debug("WriteEventChanger: connection state: " + _model.Database.Connection.State.ToString());

                DbCommand command = _model.Database.Connection.CreateCommand();

                command.CommandText = $"INSERT INTO EventChanger (IDChanger, IDEventChangerKind, DTime) " +
                    $"VALUES ((select IDChanger from Changers c where c.Name = '{eventFull.changer}'), " +
                    $"(select IDEventChangerKind from EventChangerKind evk where evk.Code = '{eventFull.eventKindCode}')," +
                    $"'{eventFull.dtime.ToString("yyyy-MM-dd HH:mm:ss")}'); " +
                    $"SELECT SCOPE_IDENTITY();";

                Logger.Log.Debug("WriteEventChanger: command is:\n" + command.CommandText);

                var id = command.ExecuteScalar();

                _model.Database.Connection.Close();

                if (id == null || int.Parse(id.ToString()) < 1)
                {
                    Logger.Log.Error("WriteEventChanger: ошибка при записи EventChanger.");
                    return new InsertFullEventChangerResult { serverMessage = "Приём ок. Ошибка записи в базу EventChanger" };
                }

                Logger.Log.Debug("WriteEventChanger: EventChanger добавлен. id = " + id.ToString());

                InsertFullEventChangerResult result = new InsertFullEventChangerResult { serverId = int.Parse(id.ToString()) };

                int idEventChanger = int.Parse(id.ToString());

                if (eventFull.eventsAcquiring.Count > 0)
                {
                    result.eventsAcquiring = new List<DbInsertResult>();
                    foreach (EventAcquiring ea in eventFull.eventsAcquiring)
                    {
                        result.eventsAcquiring.Add(WriteEventChangerAcquiring(ea, idEventChanger));
                    }
                }

                if (eventFull.eventsNominals.Count > 0)
                {
                    result.eventsNominals = new List<DbInsertResult>();
                    foreach (EventWithNominals ewn in eventFull.eventsNominals)
                    {
                        if (ewn.eventKind.Equals("increase"))
                            result.eventsNominals.Add(WriteEventChangerIncrease(ewn, idEventChanger));
                        if (ewn.eventKind.Equals("out"))
                            result.eventsNominals.Add(WriteEventChangerOut(ewn, idEventChanger));
                    }
                }

                if (eventFull.eventsCard.Count > 0)
                {
                    result.eventsCard = new List<DbInsertResult>();
                    foreach (EventCard ec in eventFull.eventsCard)
                    {
                        result.eventsCard.Add(WriteEventChangerCard(ec, idEventChanger));
                    }
                }

                return result;
            }
            catch (Exception e)
            {
                if (_model.Database.Connection.State == System.Data.ConnectionState.Open)
                    _model.Database.Connection.Close();

                Logger.Log.Error("WriteEventChanger: ошибка при записи в базу.\n" + e.Message + Environment.NewLine + e.StackTrace);
                return new InsertFullEventChangerResult { serverMessage = "Приём ок. Ошибка записи в базу" };
            }
        }

        private DbInsertResult WriteEventChangerAcquiring(EventAcquiring eventAcquiring, int idEventChanger)
        {
            Logger.InitLogger();
            try
            {
                _model.Database.Connection.Open();
                Logger.Log.Debug("WriteEventChangerAcquiring: connection state: " + _model.Database.Connection.State);

                DbCommand command = _model.Database.Connection.CreateCommand();
                command.CommandText = $"INSERT INTO EventChangerAcquiring (IDEventChanger, DTime, Amount) " +
                    $"VALUES ({idEventChanger}, '{eventAcquiring.dtime.ToString("yyyy-MM-dd HH:mm:ss")}', {eventAcquiring.amount}); " +
                    $"SELECT SCOPE_IDENTITY();";

                Logger.Log.Debug("WriteEventChangerAcquiring: command is:\n" + command.CommandText);

                var id = command.ExecuteScalar();

                _model.Database.Connection.Close();

                if (id == null || int.Parse(id.ToString()) < 1)
                {
                    Logger.Log.Error("WriteEventChangerAcquiring: ошибка при записи EventChangerAcquiring." + Environment.NewLine);
                    return new DbInsertResult { serverMessage = "Ошибка записи в базу EventChangerAcquiring" };
                }

                Logger.Log.Debug("WriteEventChangerAcquiring: eventAcquiring добавлен id = " + id.ToString() + Environment.NewLine);
                return new DbInsertResult { serverId = int.Parse(id.ToString()) };
            }
            catch (Exception e)
            {
                if (_model.Database.Connection.State == System.Data.ConnectionState.Open)
                    _model.Database.Connection.Close();
                Logger.Log.Error("WriteEventChangerAcquiring: ошибка при записи в базу. Событие:\n" + JsonConvert.SerializeObject(eventAcquiring) + Environment.NewLine + e.Message + Environment.NewLine);
                return new DbInsertResult { serverMessage = "Приём ок. Ошибка записи в базу" };
            }
        }

        private DbInsertResult WriteEventChangerCard(EventCard eventCard, int idEventChanger)
        {
            Logger.InitLogger();
            try
            {
                _model.Database.Connection.Open();
                Logger.Log.Debug("WriteEventChangerAcquiring: connection state: " + _model.Database.Connection.State);

                DbCommand command = _model.Database.Connection.CreateCommand();
                command.CommandText = $"INSERT INTO EventChangerCard (IDEventChanger, DTime, CardNum) " +
                    $"VALUES ({idEventChanger}, '{eventCard.dtime.ToString("yyyy-MM-dd HH:mm:ss")}', '{eventCard.cardNum}'); " +
                    $"SELECT SCOPE_IDENTITY();";

                Logger.Log.Debug("WriteEventChangerCard: command is:\n" + command.CommandText);

                var id = command.ExecuteScalar();

                _model.Database.Connection.Close();

                if (id == null || int.Parse(id.ToString()) < 1)
                {
                    Logger.Log.Error("WriteEventChangerCard: ошибка при записи EventChangerCard." + Environment.NewLine);
                    return new DbInsertResult { serverMessage = "Ошибка записи в базу EventChangerCard" };
                }

                Logger.Log.Debug("WriteEventChangerCard: eventCard добавлен id = " + id.ToString() + Environment.NewLine);
                return new DbInsertResult { serverId = int.Parse(id.ToString()) };
            }
            catch (Exception e)
            {
                if (_model.Database.Connection.State == System.Data.ConnectionState.Open)
                    _model.Database.Connection.Close();
                Logger.Log.Error("WriteEventChangerCard: ошибка при записи в базу. Событие:\n" + JsonConvert.SerializeObject(eventCard) + Environment.NewLine + e.Message + Environment.NewLine);
                return new DbInsertResult { serverMessage = "Приём ок. Ошибка записи в базу" };
            }
        }

        private DbInsertResult WriteEventChangerIncrease(EventWithNominals eventIncrease, int idEventChanger)
        {
            Logger.InitLogger();
            try
            {
                _model.Database.Connection.Open();
                Logger.Log.Debug("WriteEventChangerIncrease: connection state: " + _model.Database.Connection.State);

                DbCommand command = _model.Database.Connection.CreateCommand();
                command.CommandText = $"INSERT INTO EventChangerIncrease (IDEventChanger, DTime, m10, b50, b100, b200, b500, b1000, b2000) " +
                    $"VALUES ({idEventChanger}, '{eventIncrease.dtime.ToString("yyyy-MM-dd HH:mm:ss")}', {eventIncrease.m10}, {eventIncrease.b50}, {eventIncrease.b100}, {eventIncrease.b200}, " +
                    $"{eventIncrease.b500}, {eventIncrease.b1000}, {eventIncrease.b2000}); " +
                    $"SELECT SCOPE_IDENTITY();";

                Logger.Log.Debug("WriteEventChangerIncrease: command is:\n" + command.CommandText);

                var id = command.ExecuteScalar();

                _model.Database.Connection.Close();

                if (id == null || int.Parse(id.ToString()) < 1)
                {
                    Logger.Log.Error("WriteEventChangerIncrease: ошибка при записи EventChangerIncrease." + Environment.NewLine);
                    return new DbInsertResult { serverMessage = "Ошибка записи в базу EventChangerIncrease" };
                }

                Logger.Log.Debug("WriteEventChangerIncrease: eventIncrease добавлен id = " + id.ToString() + Environment.NewLine);
                return new DbInsertResult { serverId = int.Parse(id.ToString()) };
            }
            catch (Exception e)
            {
                if (_model.Database.Connection.State == System.Data.ConnectionState.Open)
                    _model.Database.Connection.Close();
                Logger.Log.Error("WriteEventChangerIncrease: ошибка при записи в базу. Событие:\n" + JsonConvert.SerializeObject(eventIncrease) + Environment.NewLine + e.Message + Environment.NewLine);
                return new DbInsertResult { serverMessage = "Приём ок. Ошибка записи в базу" };
            }
        }

        private DbInsertResult WriteEventChangerOut(EventWithNominals eventIncrease, int idEventChanger)
        {
            Logger.InitLogger();
            try
            {
                _model.Database.Connection.Open();
                Logger.Log.Debug("WriteEventChangerOut: connection state: " + _model.Database.Connection.State);

                DbCommand command = _model.Database.Connection.CreateCommand();
                command.CommandText = $"INSERT INTO EventChangerOut (IDEventChanger, DTime, m10, b50, b100) " +
                    $"VALUES ({idEventChanger}, '{eventIncrease.dtime.ToString("yyyy-MM-dd HH:mm:ss")}', {eventIncrease.m10}, {eventIncrease.b50}, {eventIncrease.b100}); " +
                    $"SELECT SCOPE_IDENTITY();";

                Logger.Log.Debug("WriteEventChangerOut: command is:\n" + command.CommandText);

                var id = command.ExecuteScalar();

                _model.Database.Connection.Close();

                if (id == null || int.Parse(id.ToString()) < 1)
                {
                    Logger.Log.Error("WriteEventChangerOut: ошибка при записи EventChangerOut." + Environment.NewLine);
                    return new DbInsertResult { serverMessage = "Ошибка записи в базу EventChangerOut" };
                }

                Logger.Log.Debug("WriteEventChangerOut: eventOut добавлен id = " + id.ToString() + Environment.NewLine);
                return new DbInsertResult { serverId = int.Parse(id.ToString()) };
            }
            catch (Exception e)
            {
                if (_model.Database.Connection.State == System.Data.ConnectionState.Open)
                    _model.Database.Connection.Close();
                Logger.Log.Error("WriteEventChangerOut: ошибка при записи в базу. Событие:\n" + JsonConvert.SerializeObject(eventIncrease) + Environment.NewLine + e.Message + Environment.NewLine);
                return new DbInsertResult { serverMessage = "Приём ок. Ошибка записи в базу" };
            }
        }
    }
}
