using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace PostControllingService.Controllers.Supplies
{
    public class SendPostResponse
    { 
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }

        public SendPostResponse(HttpStatusCode code, string res)
        {
            this.StatusCode = code;
            this.Message = res;
        }
    }
}