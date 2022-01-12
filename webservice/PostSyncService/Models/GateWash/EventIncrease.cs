﻿using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace PostSyncService.Models.GateWash
{
    public partial class EventIncrease
    {
        public int Idevent { get; set; }
        public int? Amount { get; set; }
        public int? M10 { get; set; }
        public int? B50 { get; set; }
        public int? B100 { get; set; }
        public int? B200 { get; set; }
        public int? B500 { get; set; }
        public int? B1000 { get; set; }
        public int? B2000 { get; set; }

        public virtual Event IdeventNavigation { get; set; }
    }
}
