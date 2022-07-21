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
        public int Idcondition { get; set; }
        public int Idgroup { get; set; }

        public virtual HappyHourCondition Idcondition1 { get; set; }
        public virtual HolidayCondition Idcondition2 { get; set; }
        public virtual VipCondition Idcondition3 { get; set; }
        public virtual EachNwashCondition IdconditionNavigation { get; set; }
        public virtual Group IdgroupNavigation { get; set; }
        public virtual PromotionType IdpromotionTypeNavigation { get; set; }
    }
}
