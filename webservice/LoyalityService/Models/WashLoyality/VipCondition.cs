using System;
using System.Collections.Generic;

#nullable disable

namespace LoyalityService.Models.WashLoyality
{
    public partial class VipCondition
    {
        public int Idpromotion { get; set; }
        public long Phone { get; set; }
        public int Days { get; set; }
        public int Amount { get; set; }

        public virtual Promotion IdpromotionNavigation { get; set; }
    }
}
