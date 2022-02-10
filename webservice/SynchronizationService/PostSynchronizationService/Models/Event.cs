using System;
using System.Collections.Generic;

#nullable disable

namespace PostSynchronizationService.Models
{
    public partial class Event
    {
        public int Idevent { get; set; }
        public int Idpost { get; set; }
        public int IdeventKind { get; set; }
        public DateTime Dtime { get; set; }
        public int? IdeventPost { get; set; }

        public virtual EventKind IdeventKindNavigation { get; set; }
        public virtual Post IdpostNavigation { get; set; }
        public virtual EventIncrease EventIncrease { get; set; }
    }
}
