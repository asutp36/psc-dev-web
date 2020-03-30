using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CashIntegration.Supplies
{
    public class IntegrationResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public int TransactionId { get; set; }
    }
}
