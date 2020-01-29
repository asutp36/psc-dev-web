using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MobileIntegration.Controllers.Supplies
{
    public class ChangeCardState
    {
        public string time_send { get; set; }
        public string hash { get; set; }
        public string wash_id { get; set; }
        public string operation_time { get; set; }
        public string card { get; set; }
        public string card_status { get; set; }

        public ChangeCardState(string dt, string ct, string c, string s)
        {
            time_send = dt;
            hash = CryptHash.GetHashCode(dt);
            wash_id = "M1-3";
            operation_time = ct;
            card = c;
            card_status = s;
        }
    }
}