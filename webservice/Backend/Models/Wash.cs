using System;
using System.Collections.Generic;

namespace Backend.Models
{
    public partial class Wash
    {
        public Wash()
        {
            Changers = new HashSet<Changers>();
            Posts = new HashSet<Posts>();
            UsersAvailableWash = new HashSet<UsersAvailableWash>();
        }

        public int Idwash { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public int Idregion { get; set; }

        public virtual ICollection<Changers> Changers { get; set; }
        public virtual ICollection<Posts> Posts { get; set; }
        public virtual ICollection<UsersAvailableWash> UsersAvailableWash { get; set; }
    }
}
