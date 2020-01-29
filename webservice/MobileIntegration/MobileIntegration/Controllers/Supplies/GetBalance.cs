using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MobileIntegration.Controllers.Supplies
{
    public class GetBalance
    {
        public string time_send { get; set; }
        public string hash { get; set; }
        public string card { get; set; }

        public GetBalance(string dt, string c)
        {
            this.time_send = dt;
            this.hash = CryptHash.GetHashCode(dt);
            this.card = c;
        }
    }
}