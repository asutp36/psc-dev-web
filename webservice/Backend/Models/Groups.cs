using System;
using System.Collections.Generic;

namespace Backend.Models
{
    public partial class Groups
    {
        public Groups()
        {
            TechCardSync = new HashSet<TechCardSync>();
        }

        public int Idgroup { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<TechCardSync> TechCardSync { get; set; }
    }
}
