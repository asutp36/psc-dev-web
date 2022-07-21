using System;
using System.Collections.Generic;

#nullable disable

namespace LoyalityService.Models.WashLoyality
{
    public partial class Device
    {
        public Device()
        {
            Posts = new HashSet<Post>();
            Terminals = new HashSet<Terminal>();
        }

        public int Iddevice { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int? ServerId { get; set; }
        public string IpAddress { get; set; }

        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<Terminal> Terminals { get; set; }
    }
}
