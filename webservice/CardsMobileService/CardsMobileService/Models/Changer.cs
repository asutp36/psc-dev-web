using System;
using System.Collections.Generic;

#nullable disable

namespace CardsMobileService.Models
{
    public partial class Changer
    {
        public int Idchanger { get; set; }
        public int? Iddevice { get; set; }
        public int Idwash { get; set; }

        public virtual Device IddeviceNavigation { get; set; }
        public virtual Wash IdwashNavigation { get; set; }
    }
}
