using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace PostSyncService.Models.GateWash
{
    public partial class Regions
    {
        public Regions()
        {
            Wash = new HashSet<Wash>();
        }

        public int Idregion { get; set; }
        public short Code { get; set; }
        public string Name { get; set; }
        public int Idcompany { get; set; }

        public virtual ICollection<Wash> Wash { get; set; }
    }
}
