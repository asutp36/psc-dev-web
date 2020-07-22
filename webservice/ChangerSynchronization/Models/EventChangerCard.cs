using System;
using System.Collections.Generic;

namespace ChangerSynchronization.Models
{
    public partial class EventChangerCard
    {
        public int IdeventChangerCard { get; set; }
        public int IdeventChanger { get; set; }
        public DateTime Dtime { get; set; }
        public string CardNum { get; set; }

        public virtual EventChanger IdeventChangerNavigation { get; set; }
    }
}
