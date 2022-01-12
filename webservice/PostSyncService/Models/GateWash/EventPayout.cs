using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace PostSyncService.Models.GateWash
{
    public partial class EventPayout
    {
        public int Idevent { get; set; }
        public int? Amount { get; set; }
        public int? B50 { get; set; }
        public int? B100 { get; set; }
        public int? StorageB50 { get; set; }
        public int? StorageB100 { get; set; }

        public virtual Event IdeventNavigation { get; set; }
    }
}
