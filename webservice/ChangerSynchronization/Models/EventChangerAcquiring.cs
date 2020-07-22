using System;
using System.Collections.Generic;

namespace ChangerSynchronization.Models
{
    public partial class EventChangerAcquiring
    {
        public int IdeventChangerAcquiring { get; set; }
        public int IdeventChanger { get; set; }
        public DateTime Dtime { get; set; }
        public int Amount { get; set; }

        public virtual EventChanger IdeventChangerNavigation { get; set; }
    }
}
