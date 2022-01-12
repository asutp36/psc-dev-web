using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostSyncService.Controllers
{
    public class Error
    {
        public string errorCode { get; set; }
        public string errorMessage { get; set; }

        public Error() { }
        public Error(string code, string message)
        {
            errorCode = code;
            errorMessage = message;
        }
    }
}
