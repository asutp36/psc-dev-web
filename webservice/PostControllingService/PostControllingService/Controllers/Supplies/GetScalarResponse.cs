using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace PostControllingService.Controllers.Supplies
{
    public class GetScalarResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Result { get; set; }

        public GetScalarResponse(HttpStatusCode code, string result)
        {
            this.StatusCode = code;
            this.Result = result;
        }
    }
}