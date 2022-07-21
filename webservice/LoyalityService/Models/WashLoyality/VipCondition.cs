using System;
using System.Collections.Generic;

#nullable disable

namespace LoyalityService.Models.WashLoyality
{
    public partial class VipCondition
    {
        public VipCondition()
        {
            Promotions = new HashSet<Promotion>();
        }

        public int Idcondition { get; set; }
        public long Phone { get; set; }

        public virtual ICollection<Promotion> Promotions { get; set; }
    }
}
