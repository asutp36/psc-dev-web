﻿using System;
using System.Collections.Generic;

#nullable disable

namespace GateWashDataService.Models.GateWashContext
{
    public partial class Region
    {
        public Region()
        {
            Washes = new HashSet<Wash>();
        }

        public int Idregion { get; set; }
        public short Code { get; set; }
        public string Name { get; set; }
        public int Idcompany { get; set; }

        public virtual Company IdcompanyNavigation { get; set; }
        public virtual ICollection<Wash> Washes { get; set; }
    }
}