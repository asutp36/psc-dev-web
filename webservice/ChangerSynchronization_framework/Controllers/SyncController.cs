using ChangerSynchronization_framework.Controllers.Supplies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ChangerSynchronization_framework.Controllers
{
    public class SyncController : ApiController
    {
        public HttpResponseMessage PostEventChanger([FromBody]EventChangerFull model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.NoContent);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
