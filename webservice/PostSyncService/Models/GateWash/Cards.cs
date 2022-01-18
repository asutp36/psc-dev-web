using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace PostSyncService.Models.GateWash
{
    public partial class Cards
    {
        public Cards()
        {
            Sessions = new HashSet<Sessions>();
        }

        public int Idcard { get; set; }
        public string CardNum { get; set; }

        public virtual ICollection<Sessions> Sessions { get; set; }
    }
}
