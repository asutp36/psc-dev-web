using System;
using System.Collections.Generic;

#nullable disable

namespace LoyalityService.Models.WashLoyality
{
    public partial class HolidayCondition
    {
        public HolidayCondition()
        {
            Promotions = new HashSet<Promotion>();
        }

        public int Idcondition { get; set; }
        public DateTime Date { get; set; }

        public virtual ICollection<Promotion> Promotions { get; set; }
    }
}
