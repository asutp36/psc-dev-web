using System;
using System.Collections.Generic;

namespace ChangerSynchronization.Models
{
    public partial class Changers
    {
        public Changers()
        {
            EventChanger = new HashSet<EventChanger>();
            Operations = new HashSet<Operations>();
        }

        public int Idchanger { get; set; }
        public string Name { get; set; }
        public int Idwash { get; set; }

        public virtual Wash IdwashNavigation { get; set; }
        public virtual ICollection<EventChanger> EventChanger { get; set; }
        public virtual ICollection<Operations> Operations { get; set; }
    }
}
