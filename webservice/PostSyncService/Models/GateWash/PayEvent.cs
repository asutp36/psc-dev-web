using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace PostSyncService.Models.GateWash
{
    public partial class PayEvent
    {
        public int IdpayEvent { get; set; }
        public int IdeventOnPost { get; set; }
        public int IdpaySession { get; set; }
        public int IdeventKind { get; set; }
        public int Iddevice { get; set; }
        public DateTime Dtime { get; set; }

        public virtual Device IddeviceNavigation { get; set; }
        public virtual EventKind IdeventKindNavigation { get; set; }
        public virtual PaySession IdpaySessionNavigation { get; set; }
        public virtual EventCollect EventCollect { get; set; }
        public virtual EventIncrease EventIncrease { get; set; }
        public virtual EventPayout EventPayout { get; set; }
    }
}
