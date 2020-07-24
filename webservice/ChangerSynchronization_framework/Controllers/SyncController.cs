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

                DbInsertResult eventChangerResult = WriteEventChanger(model);

                if (eventChangerResult.serverId != null && eventChangerResult.serverId > 0)
                {

                }
                else
                {
                    Logger.Log.Error("PostEventChanger: не удалось записать в базу EventChanger." + Environment.NewLine);
                    return Request.CreateResponse((HttpStatusCode)503, "Не удалось записать в базу EventChanger");
                }

                return Request.CreateResponse(HttpStatusCode.Created);
            }
            catch (Exception e)
            {
                Logger.Log.Error("PostEventChanger: " + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        private DbInsertResult WriteEventChanger(EventChangerFull eventFull)
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

                Logger.Log.Debug("WriteEventChanger: EventChanger добавлен. id = " + id.ToString());

                return new DbInsertResult { serverId = int.Parse(id.ToString()) };
            }
            catch (Exception e)
            {
                Logger.Log.Error("WriteEventChanger: ошибка при записи в базу.\n" + e.Message + Environment.NewLine + e.StackTrace);
                return new DbInsertResult { serverMassege = "Приём ок. Ошибка записи в базу" };
            }
        }
    }
}
