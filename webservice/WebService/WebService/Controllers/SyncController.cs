using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebService.Models;
using WebService.Controllers.Supplies;
//using System.Data.SqlClient;

namespace WebService.Controllers
{
    public class SyncController : ApiController
    {
        private ModelDb _model = new ModelDb();

        public string GetData()
        {
            return _model.Database.Connection.State.ToString();
        }

        private void CheckConnection()
        {
            try
            {
                _model.Database.Connection.Close();
                _model.Database.Connection.Open();
            }
            catch (Exception e)
            {
                Logger.InitLogger();
                Logger.Log.Error(e.ToString());
            }
        }

        [HttpPost]
        [ActionName("op")]
        public HttpResponseMessage PostOp([FromBody]Operation operation)
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

                        string query = "INSERT INTO Operations (IDPsc, IDOperationType, IDCard, DTime, Amount, Balance, LocalizedBy, LocalizedID)" +
                                                $" VALUES({operation.IDPsc}, {operation.IDOperationType}, {operation.IDCard}, \'{operation.DTime.ToString("yyyyMMdd HH:mm:ss")}\', {operation.Amount}, {operation.Balance}, {operation.IDPsc}, {operation.IDOperation});" +
                                                " SELECT SCOPE_IDENTITY()";

                        DbCommand command = _model.Database.Connection.CreateCommand();

                        command.CommandText = "INSERT INTO Operations (IDPsc, IDOperationType, IDCard, DTime, Amount, Balance, LocalizedBy, LocalizedID)" +
                                                $" VALUES({operation.IDPsc}, {operation.IDOperationType}, {operation.IDCard}, \'{operation.DTime.ToString("yyyyMMdd HH:mm:ss")}\', {operation.Amount}, {operation.Balance}, {operation.IDPsc}, {operation.IDOperation});" + 
                                                " SELECT SCOPE_IDENTITY()";
                        Logger.Log.Debug("Command is: " + command.CommandText);

                        // command.ExecuteNonQuery();

                        // DbCommand comm2 = _model.Database.Connection.CreateCommand();
                        // comm2.CommandText = "SELECT SCOPE_IDENTITY()";

                        // var rr = comm2.ExecuteScalar();
                        // int serverID = (System.Data.SqlDbType.Int)rr;
                        int serverID = (int)command.ExecuteScalar();

                        _model.Database.Connection.Close();

                        //Logger.Log.Debug("ServerID:" + serverID);

                        var responseGood = Request.CreateResponse(HttpStatusCode.OK);
                        //responseGood.Headers.Add("ServerID", serverID.ToString());
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

        [HttpPost]
        [ActionName("owner")]
        public HttpResponseMessage PostOw([FromBody]Owner owner)
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
                        Logger.Log.Debug("Added serverID:" + serverID);

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

        [HttpPost]
        [ActionName("card")]
        public HttpResponseMessage PostCard([FromBody]Card card)
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
              //                                  $" VALUES({card.IDOwner}, {card.CardNum}, {card.IDCardStatus}, {card.IDCardType}, {card.IDPsc}, {card.IDCard});" +
                                                " SELECT SCOPE_IDENTITY()";
                        Logger.Log.Debug("Command is: " + command.CommandText);

                        int serverID = (int)command.ExecuteScalar();

                        _model.Database.Connection.Close();

                        Logger.Log.Debug("Результат:" + serverID);

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

        [HttpGet]
        [ActionName("test")]
        public string Test(int x, int y)
        {
            int x1 = x * 2;
            //int y1 = int.Parse(y) * 2;
            return "x1=" + x1.ToString() + "y1=" + y.ToString();
        }

    }
}
