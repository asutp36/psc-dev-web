using System;
using System.Collections.Generic;

#nullable disable

namespace LoyalityService.Models.WashLoyality
{
    public partial class Client
    {
        public Client()
        {
            Washings = new HashSet<Washing>();
        }

        public int Idclient { get; set; }
        public long Phone { get; set; }

        public virtual ICollection<Washing> Washings { get; set; }
    }
}
