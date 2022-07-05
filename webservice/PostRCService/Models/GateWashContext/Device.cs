using System;
using System.Collections.Generic;

#nullable disable

namespace PostRCService.Models.GateWashContext
{
    public partial class Device
    {
        public int Iddevice { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int? ServerId { get; set; }
        public int? IddeviceType { get; set; }
        public string IpAddress { get; set; }
        public long? Phone { get; set; }

        public virtual DeviceType IddeviceTypeNavigation { get; set; }
    }
}
