using System;
using System.Collections.Generic;

namespace ChangerSynchronization.Models
{
    public partial class EventChangerIncrease
    {
        public int IdeventChangerIncrease { get; set; }
        public int IdeventChanger { get; set; }
        public DateTime Dtime { get; set; }
        public int M10 { get; set; }
        public int B50 { get; set; }
        public int B100 { get; set; }
        public int B200 { get; set; }
        public int B500 { get; set; }
        public int B1000 { get; set; }
        public int B2000 { get; set; }

        public virtual EventChanger IdeventChangerNavigation { get; set; }
    }
}
