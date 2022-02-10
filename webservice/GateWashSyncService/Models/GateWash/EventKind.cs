using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace GateWashSyncService.Models.GateWash
{
    public partial class EventKind
    {
        public EventKind()
        {
            Event = new HashSet<Event>();
            PayEvent = new HashSet<PayEvent>();
        }

        public int IdeventKind { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Event> Event { get; set; }
        public virtual ICollection<PayEvent> PayEvent { get; set; }
    }
}
