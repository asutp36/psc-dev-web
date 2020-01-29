using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MobileIntegration.Controllers.Supplies
{
    public class BalanceChanging
    {
        public string time_send { get; set; }
        public string hash { get; set; }
        public string card { get; set; }
        //public string operation_time { get; set; }
        public string post_id { get; set; }
        public int value { get; set; }

        public BalanceChanging(string dt, string h, string c, string ot, string w, int val)
        {
            this.time_send = dt;
            this.hash = CryptHash.GetHashCode(dt);
            this.card = c;
            //this.operation_time = ot;
            this.post_id = w;
            this.value = val;
        }
    }
}