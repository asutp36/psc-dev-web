using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace Inspinia_MVC5.Helpers
{
    public class GetScalarResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Result { get; set; }

        public GetScalarResponse(HttpStatusCode code, string result)
        {
            StatusCode = code;
            Result = result;
        }
    }
}