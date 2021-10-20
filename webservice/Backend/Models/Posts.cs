using System;
using System.Collections.Generic;

namespace Backend.Models
{
    public partial class Posts
    {
        public Posts()
        {
            Event = new HashSet<Event>();
        }

        public int Idpost { get; set; }
        public int Idwash { get; set; }
        public int? Iddevice { get; set; }
        public string Qrcode { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual Device IddeviceNavigation { get; set; }
        public virtual Wash IdwashNavigation { get; set; }
        public virtual ICollection<Event> Event { get; set; }
    }
}
