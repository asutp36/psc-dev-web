using System;
using System.Collections.Generic;
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
            return "sync";
        }

        [HttpPost]
        [ActionName("op")]
        public void PostOp([FromBody]Operation operation)
        {
            _model.Operations.Add(operation);
            _model.SaveChanges();
        }

        [HttpPost]
        [ActionName("owner")]
        public void PostOw([FromBody]Owner owner)
        {
            _model.Owners.Add(owner);
            _model.SaveChanges();
        }

        [HttpPost]
        [ActionName("card")]
        public void PostCard([FromBody]Card card)
        {
            _model.Cards.Add(card);
            _model.SaveChanges();
        }

    }
}
