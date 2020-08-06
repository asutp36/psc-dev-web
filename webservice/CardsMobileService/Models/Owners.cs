using System;
using System.Collections.Generic;

namespace CardsMobileService.Models
{
    public partial class Owners
    {
        public Owners()
        {
            Cards = new HashSet<Cards>();
        }

        public int Idowner { get; set; }
        public string Phone { get; set; }
        public long? PhoneInt { get; set; }
        public int LocalizedBy { get; set; }
        public int LocalizedId { get; set; }

        public virtual ICollection<Cards> Cards { get; set; }
    }
}
