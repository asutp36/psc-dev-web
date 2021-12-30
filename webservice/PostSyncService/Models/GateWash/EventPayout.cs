using System;
using System.Collections.Generic;

#nullable disable

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
