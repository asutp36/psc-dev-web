using System;
using System.Collections.Generic;

namespace ChangerSynchronization.Models
{
    public partial class Operations
    {
        public int Idoperation { get; set; }
        public int Idchanger { get; set; }
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
        public virtual Changers IdchangerNavigation { get; set; }
        public virtual OperationTypes IdoperationTypeNavigation { get; set; }
    }
}
