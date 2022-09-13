using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationService.Models
{
    public class ErrorModel
    {
        public string ErrorMessage { get; set; }
        public string ErrorType { get; set; }
        public string Alert { get; set; }
        public string ErrorCode { get; set; }
    }
}
