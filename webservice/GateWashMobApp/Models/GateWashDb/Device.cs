using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace GateWashMobApp.Models.GateWashDb
{
    public partial class Device
    {
        public Device()
        {
            Posts = new HashSet<Posts>();
            Terminals = new HashSet<Terminals>();
        }

        public int Iddevice { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int? ServerId { get; set; }
        public int? IddeviceType { get; set; }
        public string IpAddress { get; set; }

        public virtual DeviceTypes IddeviceTypeNavigation { get; set; }
        public virtual ICollection<Posts> Posts { get; set; }
        public virtual ICollection<Terminals> Terminals { get; set; }
    }
}
