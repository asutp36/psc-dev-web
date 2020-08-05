﻿using ChangerSynchronization_framework.Controllers.Supplies;
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
    [Route("api/operations")]
    public class OperationsController : ApiController
    {
        ModelDb _model = new ModelDb();

        /// <summary>
        /// Запись события об операции по карте
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <response code="201">Запись создана</response>
        /// <response code="500">Внутренняя ошибка</response>
        /// <response code="400">Модель не прошла влидацию</response>
        [HttpPost]
        public HttpResponseMessage Post([FromBody]OperationFromRequest model)
        {
            Logger.InitLogger();

            if (!ModelState.IsValid)
            {
                Logger.Log.Error("OperationsPost: модель не прошла валидацию\n" + JsonConvert.SerializeObject(model) + Environment.NewLine);
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            try
            {
                if (!_model.Database.Exists())
                {
                    Logger.Log.Error("OperationsPost: база данных не найдена!" + Environment.NewLine);
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "База данных не найдена");
                }

                _model.Database.Connection.Open();
                Logger.Log.Debug("OperationsPost: connection state: " + _model.Database.Connection.State.ToString());

                DbCommand command = _model.Database.Connection.CreateCommand();
                command.CommandText = $"INSERT INTO Operations (IDDevice, IDOperationType, IDCard, DTime, Amount, Balance, LocalizedBy, LocalizedID) " +
                    $"VALUES ((select IDDevice from Device where Code = '{model.changer}'), (select IDOperationType from OperationTypes where Code = '{model.operationType}'), " +
                    $"(select IDCard from Cards where CardNum = '{model.cardNum}'), '{model.dtime.ToString("yyyy-MM-dd HH:mm:ss")}', {model.amount}, {model.balance}, " +
                    $"(select IDDevice from Device where Code = '{model.changer}'), {model.localizedID}); " +
                    $"SELECT SCOPE_IDENTITY();";
                Logger.Log.Debug("OperationsPost: command is: " + command.CommandText);

                try
                {
                    var serverID = command.ExecuteScalar();
                    Logger.Log.Debug("OperationsPost: операция добавлена. id = " + serverID.ToString() + Environment.NewLine);

                    var response = Request.CreateResponse(HttpStatusCode.Created);
                    response.Headers.Add("ServerID", serverID.ToString());

                    return response;
                }
                catch(Exception e)
                {
                    if (_model.Database.Connection.State == System.Data.ConnectionState.Open)
                        _model.Database.Connection.Close();

                    Logger.Log.Error("CardsPost: ошибка записи в базу\n" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "Ошибка при записи в базу");
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error("OperationsPost: произошла внутренняя ошибка\n" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }
    }
}
