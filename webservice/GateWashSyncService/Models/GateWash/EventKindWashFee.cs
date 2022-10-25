using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace GateWashSyncService.Models.GateWash
{
    public partial class EventKindWashFee
    {
        public int Idwash { get; set; }
        public int IdeventKind { get; set; }
        public int Fee { get; set; }

        public virtual EventKind IdeventKindNavigation { get; set; }
        public virtual Wash IdwashNavigation { get; set; }
    }
}
