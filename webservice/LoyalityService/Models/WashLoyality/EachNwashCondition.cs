using System;
using System.Collections.Generic;

#nullable disable

namespace LoyalityService.Models.WashLoyality
{
    public partial class EachNwashCondition
    {
        public EachNwashCondition()
        {
            Promotions = new HashSet<Promotion>();
        }

        public int Idcondition { get; set; }
        public int EachN { get; set; }
        public int Days { get; set; }

        public virtual ICollection<Promotion> Promotions { get; set; }
    }
}
