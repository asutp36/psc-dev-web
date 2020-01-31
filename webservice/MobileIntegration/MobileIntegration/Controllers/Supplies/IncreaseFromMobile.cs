using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MobileIntegration.Controllers.Supplies
{
    public class IncreaseFromMobile
    {
        public DateTime time_send { get; set; }
        public string hash { get; set; }
        public string card { get; set; }
        public int value { get; set; }
        public string from { get; set; }
        public string operation_type { get; set; }
    }
}