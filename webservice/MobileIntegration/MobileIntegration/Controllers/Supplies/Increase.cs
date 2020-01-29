    using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MobileIntegration.Controllers.Supplies
{
    public class Increase
    {
        public string time_send { get; set; }
        public string hash { get; set; }
        public string wash_id { get; set; }
        public string operation_time { get; set; }
        public string card { get; set; }
        public int value { get; set; }
        public string operation_type { get; set; }

        public Increase(string dt, string c, string ot, string w, int val, string t)
        {
            this.time_send = dt;
            this.hash = CryptHash.GetHashCode(dt);
            this.card = c;
            this.operation_time = ot;
            this.wash_id = w;
            this.value = val;
            this.operation_type = t;
        }
    }
}