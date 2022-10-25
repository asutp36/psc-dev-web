using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashSyncService.Exceptions
{
    public class CustomStatusCodeException : Exception
    {
        public int ResponseStatuseCode { get; set; }

        public CustomStatusCodeException(string message, int statusCode) : base(message)
        {
            ResponseStatuseCode = statusCode;
        }
    }
}
