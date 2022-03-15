using System;
using System.Collections.Generic;

#nullable disable

namespace GateWashDataService.Models.GateWashContext
{
    public partial class Function
    {
        public Function()
        {
            PaySessions = new HashSet<PaySession>();
            Sessions = new HashSet<Session>();
        }

        public int Idfunction { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int Cost { get; set; }

        public virtual ICollection<PaySession> PaySessions { get; set; }
        public virtual ICollection<Session> Sessions { get; set; }
    }
}
