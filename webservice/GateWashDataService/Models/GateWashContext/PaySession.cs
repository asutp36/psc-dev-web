using System;
using System.Collections.Generic;

#nullable disable

namespace GateWashDataService.Models.GateWashContext
{
    public partial class PaySession
    {
        public PaySession()
        {
            PayEvents = new HashSet<PayEvent>();
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
        public virtual ICollection<PayEvent> PayEvents { get; set; }
    }
}
