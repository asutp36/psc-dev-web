using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace GateWashSyncService.Models.GateWash
{
    public partial class Device
    {
        public Device()
        {
            Collect = new HashSet<Collect>();
            Event = new HashSet<Event>();
            PayEvent = new HashSet<PayEvent>();
            PaySession = new HashSet<PaySession>();
            Posts = new HashSet<Posts>();
        }

        public int Iddevice { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int? ServerId { get; set; }
        public int? IddeviceType { get; set; }
        public string IpAddress { get; set; }

        public virtual DeviceTypes IddeviceTypeNavigation { get; set; }
        public virtual ICollection<Collect> Collect { get; set; }
        public virtual ICollection<Event> Event { get; set; }
        public virtual ICollection<PayEvent> PayEvent { get; set; }
        public virtual ICollection<PaySession> PaySession { get; set; }
        public virtual ICollection<Posts> Posts { get; set; }
    }
}
