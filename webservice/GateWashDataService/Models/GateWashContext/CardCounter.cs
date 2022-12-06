using System;
using System.Collections.Generic;

#nullable disable

namespace GateWashDataService.Models.GateWashContext
{
    public partial class CardCounter
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
