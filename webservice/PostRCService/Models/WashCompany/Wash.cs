using System;
using System.Collections.Generic;

#nullable disable

namespace PostRCService.Models
{
    public partial class Wash
    {
        public Wash()
        {
            Posts = new HashSet<Post>();
        }

        public int Idwash { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public int Idregion { get; set; }

        public virtual Region IdregionNavigation { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
    }
}
