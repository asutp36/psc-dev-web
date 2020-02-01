using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace PostControllingService.Controllers.Supplies
{
    public class SendPriceResponse
    { 
        public HttpStatusCode StatusCode { get; set; }
        public string Result { get; set; }

        public SendPriceResponse(HttpStatusCode code, string res)
        {
            this.StatusCode = code;
            this.Result = res;
        }
    }
}