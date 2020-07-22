using System;
using System.Collections.Generic;

namespace ChangerSynchronization.Models
{
    public partial class EventChangerOut
    {
        public int IdeventChangerOut { get; set; }
        public int IdeventChanger { get; set; }
        public DateTime Dtime { get; set; }
        public int M10 { get; set; }
        public int B50 { get; set; }
        public int B100 { get; set; }

        public virtual EventChanger IdeventChangerNavigation { get; set; }
    }
}
