using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebService.Models;

namespace WebService.Controllers
{
    public class SyncController : ApiController
    {
        private ModelDb _model = new Models.ModelDb();

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
                new Exception(e.ToString());
            }
        }

        [HttpPost]
        [ActionName("op")]
        public HttpResponseMessage PostOp([FromBody]Operation operation)
        {
            if (operation != null)
            {
                try
                {
                    if (_model.Database.Exists())
                    {
                        _model.Database.Connection.Open();
                        DbCommand command = _model.Database.Connection.CreateCommand();
                        command.CommandText = "INSERT INTO Operations (IDPsc, IDOperationType, IDCard, DTime, Amount, Balance, LocalizedBy, LocalizedID)" +
                                                $" VALUES({operation.IDPsc}, {operation.IDOperationType}, {operation.IDCard}, {operation.DTime}, {operation.Amount}, {operation.Balance}, {operation.IDPsc}, {operation.IDOperation});" +
                                                " SELECT SCOPE_IDENTITY()";
                        int serverID = (int)command.ExecuteScalar();
                        _model.Database.Connection.Close();
                        //_model.Operations.Add(operation);
                        //_model.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                var responseGood = Request.CreateResponse(HttpStatusCode.OK);
                responseGood.Headers.Add("ServerID", "1488");
                return responseGood;
            }

            var responseBad = Request.CreateResponse(HttpStatusCode.NoContent);
            responseBad.Headers.Add("ServerID", "1488");

            return responseBad;
        }

        [HttpPost]
        [ActionName("owner")]
        public HttpResponseMessage PostOw([FromBody]Owner owner)
        {
            if (owner != null)
            {
                _model.Owners.Add(owner);
                _model.SaveChanges();

                var responseGood = Request.CreateResponse(HttpStatusCode.OK);
                responseGood.Headers.Add("ServerID", "1488");
                return responseGood;
            }

            var responseBad = Request.CreateResponse(HttpStatusCode.NoContent);
            responseBad.Headers.Add("ServerID", "1488");

            return responseBad;

        }

        [HttpPost]
        [ActionName("card")]
        public HttpResponseMessage PostCard([FromBody]Card card)
        {
            if (card != null)
            {
                _model.Cards.Add(card);
                _model.SaveChanges();

                var responseGood = Request.CreateResponse(HttpStatusCode.OK);
                responseGood.Headers.Add("ServerID", "1488");
                return responseGood;
            }

            var responseBad = Request.CreateResponse(HttpStatusCode.NoContent);
            responseBad.Headers.Add("ServerID", "1488");

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
