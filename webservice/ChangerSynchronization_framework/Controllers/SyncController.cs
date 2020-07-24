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
                    return Request.CreateResponse(HttpStatusCode.NoContent);
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


             return Request.CreateResponse(HttpStatusCode.Created);
            }
            catch (Exception e)
            {
                Logger.Log.Error("PostEventChanger: " + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
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
                    $"VALUES ((select IDChanger from Changers c where c.Code == '{eventFull.changer}'), " +
                    $"(select IDEventChangerKind from EventChangerKind evk where evk.Code == '{eventFull.eventKindCode}')," +
                    $"'{eventFull.dtime}'); " +
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

                return null;
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
                    $"VALUES ({idEventChanger}, '{eventAcquiring.dtime}', {eventAcquiring.amount}); " +
                    $"SELECT SCOPE_IDENTITY();";

                Logger.Log.Debug("WriteEventChangerAcquiring: command is:\n" + command.CommandText);

                var id = command.ExecuteScalar();

                if (id == null || int.Parse(id.ToString()) < 1)
                {
                    Logger.Log.Error("WriteEventChanger: ошибка при записи EventChangerAcquiring.");
                    return new DbInsertResult { serverMessage = "Ошибка записи в базу EventChangerAcquiring" };
                }

                Logger.Log.Debug("WriteEventChanger: eventAcquiring добавлен id = " + id.ToString());
                return new DbInsertResult { serverId = int.Parse(id.ToString()) };
            }
            catch (Exception e)
            {
                if (_model.Database.Connection.State == System.Data.ConnectionState.Open)
                    _model.Database.Connection.Close();
                Logger.Log.Error("WriteEventChangerAcquiring: ошибка при записи в базу. Событие:\n" + JsonConvert.SerializeObject(eventAcquiring) + Environment.NewLine + e.Message);
                return new DbInsertResult { serverMessage = "Приём ок. Ошибка записи в базу" };
            }
        }
    }
}
