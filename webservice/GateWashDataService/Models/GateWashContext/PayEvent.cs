using System;
using System.Collections.Generic;

#nullable disable

namespace GateWashDataService.Models.GateWashContext
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
        public virtual EventIncrease EventIncrease { get; set; }
        public virtual EventPayout EventPayout { get; set; }
    }
}
