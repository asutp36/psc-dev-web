using System;
using System.Collections.Generic;

namespace CardsMobileService.Models
{
    public partial class Posts
    {
        public int Idpost { get; set; }
        public int Idwash { get; set; }
        public int? Iddevice { get; set; }
        public string Qrcode { get; set; }

        public virtual Device IddeviceNavigation { get; set; }
    }
}
