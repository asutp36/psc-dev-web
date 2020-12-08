using System;
using System.Collections.Generic;

#nullable disable

namespace CardsMobileService.Models
{
    public partial class Operation
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

        public virtual Card IdcardNavigation { get; set; }
        public virtual Device IddeviceNavigation { get; set; }
        public virtual OperationType IdoperationTypeNavigation { get; set; }
    }
}
