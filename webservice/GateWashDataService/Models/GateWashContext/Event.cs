using System;
using System.Collections.Generic;

#nullable disable

namespace GateWashDataService.Models.GateWashContext
{
    public partial class Event
    {
        public int Idevent { get; set; }
        public int IdeventOnPost { get; set; }
        public int Idsession { get; set; }
        public int IdeventKind { get; set; }
        public int Iddevice { get; set; }
        public DateTime Dtime { get; set; }

        public virtual Device IddeviceNavigation { get; set; }
        public virtual EventKind IdeventKindNavigation { get; set; }
        public virtual Session IdsessionNavigation { get; set; }
    }
}
