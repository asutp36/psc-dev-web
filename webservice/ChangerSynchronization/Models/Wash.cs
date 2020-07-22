using System;
using System.Collections.Generic;

namespace ChangerSynchronization.Models
{
    public partial class Wash
    {
        public Wash()
        {
            Changers = new HashSet<Changers>();
        }

        public int Idwash { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public int Idregion { get; set; }

        public virtual ICollection<Changers> Changers { get; set; }
    }
}
