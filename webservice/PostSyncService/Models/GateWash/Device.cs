using System;
using System.Collections.Generic;

#nullable disable

namespace PostSyncService.Models.GateWash
{
    public partial class Device
    {
        public Device()
        {
            Posts = new HashSet<Post>();
        }

        public int Iddevice { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int? ServerId { get; set; }
        public int? IddeviceType { get; set; }
        public string IpAddress { get; set; }

        public virtual DeviceType IddeviceTypeNavigation { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
    }
}
