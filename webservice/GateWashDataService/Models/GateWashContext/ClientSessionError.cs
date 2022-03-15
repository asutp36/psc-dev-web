using System;
using System.Collections.Generic;

#nullable disable

namespace GateWashDataService.Models.GateWashContext
{
    public partial class ClientSessionError
    {
        public int IdclientSessionError { get; set; }
        public int Iddevice { get; set; }
        public string Message { get; set; }
        public DateTime Dtime { get; set; }
        public int IdpaySession { get; set; }

        public virtual Device IddeviceNavigation { get; set; }
        public virtual PaySession IdpaySessionNavigation { get; set; }
    }
}
