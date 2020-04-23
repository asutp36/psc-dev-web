using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace CashIntegration.Controllers.Supplies
{
    public class IntegrationResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }
    }
}