using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostRCService.Models
{
    public class PayoutCashInsertionModel
    {
        public string terminalCode { get; set; }
        public int m10 { get; set; }
        public int b50 { get; set; }
        public int b100 { get; set; }
        public int userID { get; set; }
    }
}
