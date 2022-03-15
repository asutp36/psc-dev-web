using System;
using System.Collections.Generic;

#nullable disable

namespace GateWashDataService.Models.GateWashContext
{
    public partial class Session
    {
        public Session()
        {
            Events = new HashSet<Event>();
        }

        public int Idsession { get; set; }
        public int IdsessoinOnWash { get; set; }
        public int Idfunction { get; set; }
        public int Idcard { get; set; }
        public DateTime Dtime { get; set; }
        public string Uuid { get; set; }

        public virtual Card IdcardNavigation { get; set; }
        public virtual Function IdfunctionNavigation { get; set; }
        public virtual ICollection<Event> Events { get; set; }
    }
}
