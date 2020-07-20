using System;
using System.Collections.Generic;

namespace MobileIntegration_v2.Models
{
    public partial class CardOperations
    {
        public int IdcardOpration { get; set; }
        public int Idstate { get; set; }
        public int Iddevice { get; set; }
        public string CardNum { get; set; }
        public DateTime Dtime { get; set; }
    }
}
