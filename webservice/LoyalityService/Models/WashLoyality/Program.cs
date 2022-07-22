using System;
using System.Collections.Generic;

#nullable disable

namespace LoyalityService.Models.WashLoyality
{
    public partial class Program
    {
        public Program()
        {
            Washings = new HashSet<Washing>();
        }

        public int Idprogram { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int Cost { get; set; }

        public virtual ICollection<Washing> Washings { get; set; }
    }
}
