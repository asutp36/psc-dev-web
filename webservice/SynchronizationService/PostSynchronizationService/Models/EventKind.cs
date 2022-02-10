using System;
using System.Collections.Generic;

#nullable disable

namespace PostSynchronizationService.Models
{
    public partial class EventKind
    {
        public EventKind()
        {
            Events = new HashSet<Event>();
        }

        public int IdeventKind { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Event> Events { get; set; }
    }
}
