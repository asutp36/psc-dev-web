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
    [Route("api/cards")]
    public class CardsController : ApiController
    {
        ModelDb _model = new ModelDb();

        /// <summary>
        /// Запись события о выпущенной карте
        /// </summary>
        /// <param name="model"></param>
        /// <returns>ServerID в заголовке при удачной записи</returns>
        /// <response code="201">Запись создана</response>
        /// <response code="500">Внутренняя ошибка, сообщение в теле (опционально)</response>
        /// <response code="400">Модель не прошла влидацию</response>
        [HttpPost]
        public HttpResponseMessage Post([FromBody]NewCard model)
        {
            Logger.InitLogger();

            if (!ModelState.IsValid)
            {
                Logger.Log.Error("CardsPost: модель не прошла валидацию\n" + JsonConvert.SerializeObject(model) + Environment.NewLine);
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            Logger.Log.Debug("CardsPost: запуск с параметрами: " + JsonConvert.SerializeObject(model));

            try
            {
                if (!_model.Database.Exists())
                {
                    Logger.Log.Error("CardsPost: база данных не найдена!" + Environment.NewLine);
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "База данных не найдена");
                }

                _model.Database.Connection.Open();
                Logger.Log.Debug("CardsPost: connection state: " + _model.Database.Connection.State.ToString());

                PhoneFormatter formattedPhone = new PhoneFormatter(model.phone);

                DbCommand command = _model.Database.Connection.CreateCommand();
                DbTransaction tran = _model.Database.Connection.BeginTransaction();

                command.Transaction = tran;

                try
                {
                    command.CommandText = $"insert into Owners (Phone, PhoneInt, LocalizedBy, LocalizedID) values ('{formattedPhone.phone}', {formattedPhone.phoneInt}, " +
                        $"(select IDDevice from Device where Code = '{model.changer}'), {model.localizedID})";
                    command.ExecuteNonQuery();

                    command.CommandText = $"insert into Cards (IDOwner, CardNum,  IDCardStatus, IDCardType, LocalizedBy, LocalizedID) " +
                        $"values (scope_identity(), '{model.cardNum}', (select IDCardStatus from CardStatuses where Code = 'norm'), " +
                        $"(select IDCardType from CardTypes where Code = 'client'), (select IDDevice from Device where Code = '{model.changer}'), {model.localizedID})";
                    command.ExecuteNonQuery();

                    //command.CommandText = $"insert into Operations (IDDevice, IDOperationType, IDCard, DTime, Amount, Balance, LocalizedBy, LocalizedID) " +
                    //       $"values ((select IDDevice from Device where Code = 'MOB-EM'), " +
                    //       $"(select IDOperationType from OperationTypes ot where ot.Code = 'activation'), " +
                    //       $"scope_identity(), '{newCard.time_send}', " +
                    //       $"0, 0, (select IDDevice from Device where Code = 'MOB-EM'), 0);";
                    //command.ExecuteNonQuery();
                    
                    Logger.Log.Debug($"CadsPost: добавлены Owner и Card. CardNum = {model.cardNum}" + Environment.NewLine);
                    tran.Commit();
                }
                catch (Exception e)
                {
                    if (_model.Database.Connection.State == System.Data.ConnectionState.Open)
                        _model.Database.Connection.Close();

                    Logger.Log.Error("CardsPost: ошибка транзакции\n" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "Ошибка при записи в базу");
                }

                command.CommandText = "SELECT SCOPE_IDENTIY();";
                var serverID = command.ExecuteScalar();
                
                var response = Request.CreateResponse(HttpStatusCode.Created);
                response.Headers.Add("ServerID", serverID.ToString());

                return response;
            }
            catch (Exception e)
            {
                if (_model.Database.Connection.State == System.Data.ConnectionState.Open)
                    _model.Database.Connection.Close();

                Logger.Log.Error("CardsPost: произошла внутренняя ошибка\n" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }
    }
}
