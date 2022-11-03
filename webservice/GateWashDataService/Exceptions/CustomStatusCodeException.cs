using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace GateWashDataService.Exceptions
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
    }
}
