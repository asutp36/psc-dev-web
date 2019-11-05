using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebService.Models;
using WebService.Controllers.Supplies;
using System.IO;
using System.Web;
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

                        //string query = "INSERT INTO Operations (IDPsc, IDOperationType, IDCard, DTime, Amount, Balance, LocalizedBy, LocalizedID)" +
                        //                        $" VALUES({operation.IDPsc}, {operation.IDOperationType}, {operation.IDCard}, \'{operation.DTime.ToString("yyyyMMdd HH:mm:ss")}\', {operation.Amount}, {operation.Balance}, {operation.IDPsc}, {operation.IDOperation});" +
                        //                        " SELECT SCOPE_IDENTITY()";

                        DbCommand command = _model.Database.Connection.CreateCommand();

                        command.CommandText = "INSERT INTO Operations (IDCard, IDPsc, IDOperationType, DTime, Amount, Balance, LocalizedBy, LocalizedID)" +
                                                $" VALUES((select IDCard from Cards where LocalizedBy =  {operation.LocalizedBy} and LocalizedID = {operation.IDCard}), " +
                                                $"{operation.LocalizedBy}, {operation.IDOperationType}, \'{operation.DTime.ToString("yyyyMMdd HH:mm:ss")}\', {operation.Amount}, {operation.Balance}, {operation.LocalizedBy}, {operation.IDOperation});" + 
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

        [HttpPost]
        [ActionName("finance")]
        public HttpResponseMessage PostFinance([FromBody]FinanceJson finance)
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
                                                $" (select IDFinanceType from FinanceType where ServerID = {finance.FinanceTypeServerID}), \'{finance.DTime.ToString("yyyyMMdd HH:mm:ss")}\'," +
                                                $" {finance.Amount}); SELECT SCOPE_IDENTITY()";

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
    }
}
