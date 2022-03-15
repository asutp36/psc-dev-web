using System;
using System.Collections.Generic;

#nullable disable

namespace GateWashDataService.Models.GateWashContext
{
    public partial class PaySession
    {
        public PaySession()
        {
            ClientSessionErrors = new HashSet<ClientSessionError>();
            PayEvents = new HashSet<PayEvent>();
        }

        public int IdpaySession { get; set; }
        public int IdsessionOnPost { get; set; }
        public int Idfunction { get; set; }
        public DateTime Dtime { get; set; }
        public int Iddevice { get; set; }
        public int ProgramCost { get; set; }

        public virtual Device IddeviceNavigation { get; set; }
        public virtual Function IdfunctionNavigation { get; set; }
        public virtual ICollection<ClientSessionError> ClientSessionErrors { get; set; }
        public virtual ICollection<PayEvent> PayEvents { get; set; }
    }
}
