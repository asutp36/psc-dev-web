using System;
using System.Collections.Generic;

namespace CardsMobileService.Models
{
    public partial class Changers
    {
        public int Idchanger { get; set; }
        public int? Iddevice { get; set; }
        public int Idwash { get; set; }

        public virtual Device IddeviceNavigation { get; set; }
    }
}
