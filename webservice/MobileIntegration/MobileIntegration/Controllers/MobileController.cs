using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace MobileIntegration.Controllers
{
    public class MobileController : ApiController
    {
        private ModelDb _model = new ModelDb();

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
        [ActionName("get_balance")]
        //как высчитывать баланс карты?
        public HttpResponseMessage GetBalance([FromBody]GetBalanceMobile getBalance)
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
        public HttpResponseMessage ChangePhone([FromBody]ChPhoneMobile change)
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
    }
}
