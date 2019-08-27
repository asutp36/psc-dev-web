using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebService.Models;

namespace WebService.Controllers
{
    public class Coordinate { public int x { get; set; } public int y { get; set; } }


    public class SyncController : ApiController
    {
        private ModelDb _model = new Models.ModelDb();

        public string GetData()
        {
            return "sync";
        }

        [HttpPost]
        [ActionName("op")]
        public HttpResponseMessage PostOp([FromBody]Operation operation)
        {
            if (operation != null)
            {
                //_model.Operations.Add(operation);
                //_model.SaveChanges();

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
                //_model.Owners.Add(owner);
                //_model.SaveChanges();

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
                //_model.Cards.Add(card);
                //_model.SaveChanges();

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
