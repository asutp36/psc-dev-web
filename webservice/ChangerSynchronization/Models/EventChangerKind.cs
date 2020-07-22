using System;
using System.Collections.Generic;

namespace ChangerSynchronization.Models
{
    public partial class EventChangerKind
    {
        public EventChangerKind()
        {
            EventChanger = new HashSet<EventChanger>();
        }

        public int IdeventChangerKind { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<EventChanger> EventChanger { get; set; }
    }
}
