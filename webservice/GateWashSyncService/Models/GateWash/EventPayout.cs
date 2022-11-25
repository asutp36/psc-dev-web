using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace GateWashSyncService.Models.GateWash
{
    public partial class EventPayout
    {
        public int IdpayEvent { get; set; }
        public int Amount { get; set; }
        public int M10 { get; set; }
        public int B50 { get; set; }
        public int B100 { get; set; }
        public int Inbox1B50 { get; set; }
        public int Inbox2B50 { get; set; }
        public int Inbox3B100 { get; set; }
        public int Inbox4B100 { get; set; }
        public int Inbox5M10 { get; set; }
        public int? UserId { get; set; }

        public virtual PayEvent IdpayEventNavigation { get; set; }
    }
}
