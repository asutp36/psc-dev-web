using System;
using System.Collections.Generic;

#nullable disable

namespace LoyalityService.Models.WashLoyality
{
    public partial class PromotionType
    {
        public PromotionType()
        {
            Promotions = new HashSet<Promotion>();
        }

        public int IdpromotionType { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Promotion> Promotions { get; set; }
    }
}
