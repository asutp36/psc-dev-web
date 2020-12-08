using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardsMobileService.Controllers.Supplies
{
    public class IncreaseToApp
    {
        public string time_send { get; set; }
        public string hash { get; set; }
        public string card { get; set; }
        public int value { get; set; }
        public string wash_id { get; set; }
        public string operation_time { get; set; }
    }
}
