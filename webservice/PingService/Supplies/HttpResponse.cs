﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace PingService.Supplies
{
    public class HttpResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public string ResultMessage { get; set; }
    }
}
