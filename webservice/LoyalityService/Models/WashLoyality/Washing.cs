using System;
using System.Collections.Generic;

#nullable disable

namespace LoyalityService.Models.WashLoyality
{
    public partial class Washing
    {
        public int Idwashing { get; set; }
        public int Idclient { get; set; }
        public int Idterminal { get; set; }
        public DateTime Dtime { get; set; }
        public int Discount { get; set; }

        public virtual Client IdclientNavigation { get; set; }
        public virtual Terminal IdterminalNavigation { get; set; }
    }
}
