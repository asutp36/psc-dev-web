using System;
using System.Collections.Generic;

#nullable disable

namespace LoyalityService.Models.WashLoyality
{
    public partial class Promotion
    {
        public int Idpromotion { get; set; }
        public int IdpromotionType { get; set; }
        public int Discount { get; set; }
        public int Idgroup { get; set; }
        public int ApplyOrder { get; set; }

        public virtual Group IdgroupNavigation { get; set; }
        public virtual PromotionType IdpromotionTypeNavigation { get; set; }
        public virtual EachNwashCondition EachNwashCondition { get; set; }
        public virtual HappyHourCondition HappyHourCondition { get; set; }
        public virtual HolidayCondition HolidayCondition { get; set; }
        public virtual VipCondition VipCondition { get; set; }
    }
}
