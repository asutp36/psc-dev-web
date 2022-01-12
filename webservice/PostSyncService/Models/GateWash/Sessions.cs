using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace PostSyncService.Models.GateWash
{
    public partial class Sessions
    {
        public Sessions()
        {
            Event = new HashSet<Event>();
        }

        public int Idsession { get; set; }
        public int IdsessoinOnWash { get; set; }
        public int Iddevice { get; set; }
        public int Idfunction { get; set; }
        public string Idcard { get; set; }
        public DateTime Dtime { get; set; }
        public string Uuid { get; set; }

        public virtual ICollection<Event> Event { get; set; }
    }
}
