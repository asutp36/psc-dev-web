using System;
using System.Collections.Generic;

#nullable disable

namespace LoyalityService.Models.WashLoyality
{
    public partial class Washing
    {
        public int Idwashing { get; set; }
        public int Idclient { get; set; }
        public int Iddevice { get; set; }
        public DateTime Dtime { get; set; }
        public int Idprogram { get; set; }
        public int Amount { get; set; }
        public int Discount { get; set; }
        public int? DiscountRub { get; set; }
        public Guid? Guid { get; set; }

        public virtual Client IdclientNavigation { get; set; }
        public virtual Device IddeviceNavigation { get; set; }
        public virtual Program IdprogramNavigation { get; set; }
    }
}
