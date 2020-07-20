using System;
using System.Collections.Generic;

namespace MobileIntegration_v2.Models
{
    public partial class Posts
    {
        public int Idpost { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int Idwash { get; set; }
        public int? Iddevice { get; set; }
        public string Qrcode { get; set; }

        public virtual Device IddeviceNavigation { get; set; }
    }
}
