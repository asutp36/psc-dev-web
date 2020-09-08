using System;
using System.Collections.Generic;

namespace Backend.Models
{
    public partial class Regions
    {
        public Regions()
        {
            Wash = new HashSet<Wash>();
        }

        public int Idregion { get; set; }
        public short Code { get; set; }
        public string Name { get; set; }
        public int Idcompany { get; set; }

        public virtual ICollection<Wash> Wash { get; set; }
    }
}
