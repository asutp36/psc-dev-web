using System;
using System.Collections.Generic;

#nullable disable

namespace GateWashDataService.Models.GateWashContext
{
    public partial class EventKindWash
    {
        public int IdeventKind { get; set; }
        public int Idwash { get; set; }

        public virtual EventKind IdeventKindNavigation { get; set; }
        public virtual Wash IdwashNavigation { get; set; }
    }
}
