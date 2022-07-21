using System;
using System.Collections.Generic;

#nullable disable

namespace LoyalityService.Models.WashLoyality
{
    public partial class HolidayCondition
    {
        public int Idpromotion { get; set; }
        public DateTime Date { get; set; }

        public virtual Promotion IdpromotionNavigation { get; set; }
    }
}
