using System;
using System.Collections.Generic;

#nullable disable

namespace LoyalityService.Models.GateWashContext
{
    public partial class Washing
    {
        public int Idwashing { get; set; }
        public DateTime Dtime { get; set; }
        public long Phone { get; set; }
        public int Iddevice { get; set; }
        public bool Complited { get; set; }
        public int Discount { get; set; }
        public int PayAmount { get; set; }

        public virtual Device IddeviceNavigation { get; set; }
    }
}
