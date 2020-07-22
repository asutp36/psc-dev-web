using System;
using System.Collections.Generic;

namespace ChangerSynchronization.Models
{
    public partial class Cards
    {
        public Cards()
        {
            Operations = new HashSet<Operations>();
        }

        public int Idcard { get; set; }
        public int Idowner { get; set; }
        public string CardNum { get; set; }
        public int IdcardStatus { get; set; }
        public int IdcardType { get; set; }
        public int LocalizedBy { get; set; }
        public int LocalizedId { get; set; }

        public virtual Owners IdownerNavigation { get; set; }
        public virtual ICollection<Operations> Operations { get; set; }
    }
}
