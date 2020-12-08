using System;
using System.Collections.Generic;

#nullable disable

namespace CardsMobileService.Models
{
    public partial class Device
    {
        public Device()
        {
            Changers = new HashSet<Changer>();
            Operations = new HashSet<Operation>();
            Posts = new HashSet<Post>();
        }

        public int Iddevice { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int? ServerId { get; set; }
        public int? IddeviceType { get; set; }
        public string IpAddress { get; set; }

        public virtual DeviceType IddeviceTypeNavigation { get; set; }
        public virtual ICollection<Changer> Changers { get; set; }
        public virtual ICollection<Operation> Operations { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
    }
}
