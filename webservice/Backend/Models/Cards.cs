using System;
using System.Collections.Generic;

namespace Backend.Models
{
    public partial class Cards
    {
        public Cards()
        {
            TechCardSync = new HashSet<TechCardSync>();
        }

        public int Idcard { get; set; }
        public int Idowner { get; set; }
        public string CardNum { get; set; }
        public int IdcardStatus { get; set; }
        public int IdcardType { get; set; }
        public int? Balance { get; set; }
        public int LocalizedBy { get; set; }
        public int LocalizedId { get; set; }

        public virtual CardStatuses IdcardStatusNavigation { get; set; }
        public virtual CardTypes IdcardTypeNavigation { get; set; }
        public virtual ICollection<TechCardSync> TechCardSync { get; set; }
    }
}
