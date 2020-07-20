using System;
using System.Collections.Generic;

namespace MobileIntegration_v2.Models
{
    public partial class Owners
    {
        public Owners()
        {
            Cards = new HashSet<Cards>();
        }

        public int Idowner { get; set; }
        public string Phone { get; set; }
        public int LocalizedBy { get; set; }
        public int LocalizedId { get; set; }

        public virtual ICollection<Cards> Cards { get; set; }
    }
}
