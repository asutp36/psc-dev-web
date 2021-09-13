using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace PostControllingService.Controllers.Supplies
{
    public class HttpSenderResponse
    { 
        public HttpStatusCode StatusCode { get; set; }
        public string ResultMessage { get; set; }

        public HttpSenderResponse(HttpStatusCode code, string res)
        {
            this.StatusCode = code;
            this.ResultMessage = res;
        }
    }
}