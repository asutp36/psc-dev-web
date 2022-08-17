using System;
using System.Collections.Generic;

#nullable disable

namespace GateWashDataService.Models.GateWashContext
{
    public partial class EventKind
    {
        public EventKind()
        {
            Events = new HashSet<Event>();
            PayEvents = new HashSet<PayEvent>();
        }

        public int IdeventKind { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int DisplayOrder { get; set; }
        public float? Fee { get; set; }

        public virtual ICollection<Event> Events { get; set; }
        public virtual ICollection<PayEvent> PayEvents { get; set; }
    }
}
