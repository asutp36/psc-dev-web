using System;
using System.Collections.Generic;

namespace Backend.Models
{
    public partial class Event
    {
        public int Idevent { get; set; }
        public int Idpost { get; set; }
        public int IdeventKind { get; set; }
        public DateTime Dtime { get; set; }
        public int? IdeventPost { get; set; }

        public virtual Posts IdpostNavigation { get; set; }
        public virtual EventIncrease EventIncrease { get; set; }
    }
}
