using System;
using System.Collections.Generic;

namespace Backend.Models
{
    public partial class Operations
    {
        public int Idoperation { get; set; }
        public int? Iddevice { get; set; }
        public int IdoperationType { get; set; }
        public int Idcard { get; set; }
        public DateTime Dtime { get; set; }
        public int Amount { get; set; }
        public int Balance { get; set; }
        public int LocalizedBy { get; set; }
        public int LocalizedId { get; set; }
        public string Functions { get; set; }
        public string Details { get; set; }

        public virtual Cards IdcardNavigation { get; set; }
        public virtual Device IddeviceNavigation { get; set; }
        public virtual OperationTypes IdoperationTypeNavigation { get; set; }
    }
}
