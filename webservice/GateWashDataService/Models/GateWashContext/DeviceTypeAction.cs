using System;
using System.Collections.Generic;

#nullable disable

namespace GateWashDataService.Models.GateWashContext
{
    public partial class DeviceTypeAction
    {
        public int IddeviceType { get; set; }
        public bool InsertPayoutCash { get; set; }
        public bool InsertWashCards { get; set; }

        public virtual DeviceType IddeviceTypeNavigation { get; set; }
    }
}
