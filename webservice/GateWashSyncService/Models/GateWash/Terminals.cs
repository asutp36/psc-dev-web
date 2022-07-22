using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace GateWashSyncService.Models.GateWash
{
    public partial class Terminals
    {
        public int Idterminal { get; set; }
        public string Name { get; set; }
        public int Idwash { get; set; }
        public int Iddevice { get; set; }
        public string Qrcode { get; set; }
        public long? Phone { get; set; }

        public virtual Device IddeviceNavigation { get; set; }
        public virtual Wash IdwashNavigation { get; set; }
    }
}
