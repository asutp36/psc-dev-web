﻿using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace PostSyncService.Models.GateWash
{
    public partial class Wash
    {
        public Wash()
        {
            Posts = new HashSet<Posts>();
        }

        public int Idwash { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public int Idregion { get; set; }

        public virtual Regions IdregionNavigation { get; set; }
        public virtual ICollection<Posts> Posts { get; set; }
    }
}
