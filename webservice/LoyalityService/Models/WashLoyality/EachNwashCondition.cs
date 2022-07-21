using System;
using System.Collections.Generic;

#nullable disable

namespace LoyalityService.Models.WashLoyality
{
    public partial class EachNwashCondition
    {
        public int Idpromotion { get; set; }
        public int EachN { get; set; }
        public int Days { get; set; }

        public virtual Promotion IdpromotionNavigation { get; set; }
    }
}
