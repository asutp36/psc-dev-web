using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebService.Controllers.Supplies
{
    public class FinanceJson
    {
        public int DeviceServerID { get; set; }
        public int FinanceTypeServerID { get; set; }
        public System.DateTime DTime { get; set; }
        public int Amount { get; set; }
    }
}