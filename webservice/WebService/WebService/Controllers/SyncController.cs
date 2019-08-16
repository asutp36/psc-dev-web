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

        public void PostOp([FromBody]Operation operation)
        {
            _model.Operations.Add(operation);
            _model.SaveChanges();
            
        }

    }
}
