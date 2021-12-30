using System;
using System.Collections.Generic;

#nullable disable

namespace PostSyncService.Models.GateWash
{
    public partial class Event
    {
        public int Idevent { get; set; }
        public int Idsession { get; set; }
        public int Idpost { get; set; }
        public int IdeventKind { get; set; }
        public DateTime Dtime { get; set; }

        public virtual EventKind IdeventKindNavigation { get; set; }
        public virtual Post IdpostNavigation { get; set; }
        public virtual Session IdsessionNavigation { get; set; }
        public virtual EventIncrease EventIncrease { get; set; }
        public virtual EventPayout EventPayout { get; set; }
    }
}
