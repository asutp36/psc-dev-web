using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace GateWashSyncService.Exceptions
{
    public class CustomStatusCodeException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Description { get; set; }

        public CustomStatusCodeException(HttpStatusCode statusCode, string message, string description) : base(message)
        {
            StatusCode = statusCode;
            Description = description;
        }

        public CustomStatusCodeException(string message, int statusCode) : base(message)
        {
            StatusCode = (HttpStatusCode)statusCode;
            Description = message;
        }
    }
}
