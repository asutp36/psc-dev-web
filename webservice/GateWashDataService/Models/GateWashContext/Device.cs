using System;
using System.Collections.Generic;

#nullable disable

namespace GateWashDataService.Models.GateWashContext
{
    public partial class Device
    {
        public Device()
        {
            ClientSessionErrors = new HashSet<ClientSessionError>();
            Collects = new HashSet<Collect>();
            Events = new HashSet<Event>();
            PayEvents = new HashSet<PayEvent>();
            PaySessions = new HashSet<PaySession>();
            Posts = new HashSet<Post>();
        }

        public int Iddevice { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int? ServerId { get; set; }
        public int? IddeviceType { get; set; }
        public string IpAddress { get; set; }

        public virtual DeviceType IddeviceTypeNavigation { get; set; }
        public virtual ICollection<ClientSessionError> ClientSessionErrors { get; set; }
        public virtual ICollection<Collect> Collects { get; set; }
        public virtual ICollection<Event> Events { get; set; }
        public virtual ICollection<PayEvent> PayEvents { get; set; }
        public virtual ICollection<PaySession> PaySessions { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
    }
}
