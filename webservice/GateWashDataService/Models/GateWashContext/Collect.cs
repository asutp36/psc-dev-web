using System;
using System.Collections.Generic;

#nullable disable

namespace GateWashDataService.Models.GateWashContext
{
    public partial class Collect
    {
        public int Idcollect { get; set; }
        public DateTime Dtime { get; set; }
        public int Iddevice { get; set; }
        public int IdcollectOnPost { get; set; }
        public int Amount { get; set; }
        public int M10 { get; set; }
        public int B50 { get; set; }
        public int B100 { get; set; }
        public int B200 { get; set; }
        public int B500 { get; set; }
        public int B1000 { get; set; }
        public int B2000 { get; set; }
        public int BoxB50 { get; set; }
        public int BoxB100 { get; set; }
        public int InboxB50 { get; set; }
        public int InboxB100 { get; set; }

        public virtual Device IddeviceNavigation { get; set; }
    }
}
