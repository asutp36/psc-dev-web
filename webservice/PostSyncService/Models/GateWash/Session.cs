using System;
using System.Collections.Generic;

#nullable disable

namespace PostSyncService.Models.GateWash
{
    public partial class Session
    {
        public Session()
        {
            Events = new HashSet<Event>();
        }

        public int Idsession { get; set; }
        public int IdsessoinOnWash { get; set; }
        public int Iddevice { get; set; }
        public int Idfunction { get; set; }
        public string Idcard { get; set; }
        public DateTime Dtime { get; set; }
        public string Uuid { get; set; }

        public virtual ICollection<Event> Events { get; set; }
    }
}
