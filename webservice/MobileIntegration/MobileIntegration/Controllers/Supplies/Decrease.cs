using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MobileIntegration.Controllers.Supplies
{
    public class Decrease
    {
        public string time_send { get; set; }
        public string hash { get; set; }
        public string wash_id { get; set; }
        public string operation_time { get; set; }
        public string card { get; set; }
        public int value { get; set; }

        public Decrease(string dt, string c, string ot, string w, int val)
        {
            this.time_send = dt;
            this.hash = CryptHash.GetHashCode(dt);
            this.card = c;
            this.operation_time = ot;
            this.wash_id = w;
            this.value = val;
        }
    }
}