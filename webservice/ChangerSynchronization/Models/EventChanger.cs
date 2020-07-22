using System;
using System.Collections.Generic;

namespace ChangerSynchronization.Models
{
    public partial class EventChanger
    {
        public EventChanger()
        {
            EventChangerAcquiring = new HashSet<EventChangerAcquiring>();
            EventChangerCard = new HashSet<EventChangerCard>();
            EventChangerIncrease = new HashSet<EventChangerIncrease>();
            EventChangerOut = new HashSet<EventChangerOut>();
        }

        public int IdeventChanger { get; set; }
        public int Idchanger { get; set; }
        public int IdeventChangerKind { get; set; }
        public DateTime Dtime { get; set; }

        public virtual Changers IdchangerNavigation { get; set; }
        public virtual EventChangerKind IdeventChangerKindNavigation { get; set; }
        public virtual ICollection<EventChangerAcquiring> EventChangerAcquiring { get; set; }
        public virtual ICollection<EventChangerCard> EventChangerCard { get; set; }
        public virtual ICollection<EventChangerIncrease> EventChangerIncrease { get; set; }
        public virtual ICollection<EventChangerOut> EventChangerOut { get; set; }
    }
}
