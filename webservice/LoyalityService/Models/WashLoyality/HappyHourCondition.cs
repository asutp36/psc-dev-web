using System;
using System.Collections.Generic;

#nullable disable

namespace LoyalityService.Models.WashLoyality
{
    public partial class HappyHourCondition
    {
        public HappyHourCondition()
        {
            Promotions = new HashSet<Promotion>();
        }

        public int Idcondition { get; set; }
        public int HourBegin { get; set; }
        public int HourEnd { get; set; }

        public virtual ICollection<Promotion> Promotions { get; set; }
    }
}
