using System;
using System.Collections.Generic;

#nullable disable

namespace LoyalityService.Models.WashLoyality
{
    public partial class Group
    {
        public Group()
        {
            Promotions = new HashSet<Promotion>();
            Washes = new HashSet<Wash>();
        }

        public int Idgroup { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Promotion> Promotions { get; set; }
        public virtual ICollection<Wash> Washes { get; set; }
    }
}
