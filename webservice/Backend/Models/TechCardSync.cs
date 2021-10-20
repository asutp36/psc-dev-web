using System;
using System.Collections.Generic;

namespace Backend.Models
{
    public partial class TechCardSync
    {
        public int IdtechCardSync { get; set; }
        public int Idgroup { get; set; }
        public int Idcard { get; set; }
        public int Iddevice { get; set; }
        public DateTime LastSync { get; set; }
        public string Result { get; set; }

        public virtual Cards IdcardNavigation { get; set; }
        public virtual Device IddeviceNavigation { get; set; }
        public virtual Groups IdgroupNavigation { get; set; }
    }
}
