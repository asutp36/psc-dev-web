using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace GateWashSyncService.Models.GateWash
{
    public partial class CardCounters
    {
        public int IdcardRefill { get; set; }
        public int IdcardOperation { get; set; }
        public int Iddevice { get; set; }
        public int? IdeventKind { get; set; }
        public DateTime Dtime { get; set; }
        public int Dispenser1 { get; set; }
        public int Dispenser2 { get; set; }
        public int Count1 { get; set; }
        public int Count2 { get; set; }
        public string Login { get; set; }

        public virtual Device IddeviceNavigation { get; set; }
        public virtual EventKind IdeventKindNavigation { get; set; }
    }
}
