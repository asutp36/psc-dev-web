using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace GateWashSyncService.Models.GateWash
{
    public partial class PaySession
    {
        public PaySession()
        {
            PayEvent = new HashSet<PayEvent>();
        }

        public int IdpaySession { get; set; }
        public int IdsessionOnPost { get; set; }
        public int Idprogram { get; set; }
        public DateTime DtimeBegin { get; set; }
        public int Iddevice { get; set; }
        public int ProgramCost { get; set; }
        public string Qr { get; set; }
        public string FiscalError { get; set; }
        public DateTime? DtimeEnd { get; set; }
        public string Details { get; set; }
        public string Uuid { get; set; }

        public virtual Device IddeviceNavigation { get; set; }
        public virtual Program IdprogramNavigation { get; set; }
        public virtual ICollection<PayEvent> PayEvent { get; set; }
    }
}
