using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace PostSyncService.Models.GateWash
{
    public partial class Functions
    {
        public Functions()
        {
            PaySession = new HashSet<PaySession>();
            Sessions = new HashSet<Sessions>();
        }

        public int Idfunction { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int Cost { get; set; }

        public virtual ICollection<PaySession> PaySession { get; set; }
        public virtual ICollection<Sessions> Sessions { get; set; }
    }
}
