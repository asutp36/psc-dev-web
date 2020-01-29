using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MobileIntegration.Controllers.Supplies
{
    public class NewCard
    {
        public string time_send { get; set; }
        public string hash { get; set; }
        public string card { get; set; }
        //public string card_type { get; set; }
        public string phone { get; set; }

        public NewCard(string dt, string h, string p, string c)
        {
            this.time_send = dt;
            this.hash = CryptHash.GetHashCode(dt);
            this.phone = p;
            //this.card_type = p;
            this.card = c;
        }
    }
}