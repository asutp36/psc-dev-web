using System;
using System.Collections.Generic;

#nullable disable

namespace LoyalityService.Models.WashLoyality
{
    public partial class HappyHourCondition
    {
        public int Idpromotion { get; set; }
        public int HourBegin { get; set; }
        public int HourEnd { get; set; }

        public virtual Promotion IdpromotionNavigation { get; set; }
    }
}
