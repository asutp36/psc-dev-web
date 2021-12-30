using System;
using System.Collections.Generic;

#nullable disable

namespace PostSyncService.Models.GateWash
{
    public partial class Post
    {
        public Post()
        {
            Events = new HashSet<Event>();
        }

        public int Idpost { get; set; }
        public string Name { get; set; }
        public int Idwash { get; set; }
        public int? Iddevice { get; set; }
        public string Qrcode { get; set; }

        public virtual Device IddeviceNavigation { get; set; }
        public virtual Wash IdwashNavigation { get; set; }
        public virtual ICollection<Event> Events { get; set; }
    }
}
