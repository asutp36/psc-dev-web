using System;
using System.Collections.Generic;

namespace Backend.Models
{
    public partial class DeviceTypes
    {
        public DeviceTypes()
        {
            Device = new HashSet<Device>();
        }

        public int IddeviceType { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Device> Device { get; set; }
    }
}
