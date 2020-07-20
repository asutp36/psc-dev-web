using System;
using System.Collections.Generic;

namespace MobileIntegration_v2.Models
{
    public partial class Device
    {
        public Device()
        {
            Posts = new HashSet<Posts>();
        }

        public int Iddevice { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int? ServerId { get; set; }
        public int? IddeviceType { get; set; }
        public string IpAddress { get; set; }

        public virtual DeviceTypes IddeviceTypeNavigation { get; set; }
        public virtual ICollection<Posts> Posts { get; set; }
    }
}
