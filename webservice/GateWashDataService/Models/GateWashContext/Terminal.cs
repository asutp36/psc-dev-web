using System;
using System.Collections.Generic;

#nullable disable

namespace GateWashDataService.Models.GateWashContext
{
    public partial class Terminal
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
