using System;
using System.Collections.Generic;

#nullable disable

namespace PostSynchronizationService.Models
{
    public partial class PostSession
    {
        public PostSession()
        {
            EventIncreases = new HashSet<EventIncrease>();
        }

        public int IdpostSession { get; set; }
        public int Idpost { get; set; }
        public int IdsessionOnPost { get; set; }
        public DateTime StartDtime { get; set; }
        public string Qr { get; set; }
        public string FiscalError { get; set; }
        public DateTime StopDtime { get; set; }
        public int AmountCash { get; set; }
        public int AmountBank { get; set; }

        public virtual ICollection<EventIncrease> EventIncreases { get; set; }
    }
}
