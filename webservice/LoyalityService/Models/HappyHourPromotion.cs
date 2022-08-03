using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoyalityService.Models
{
    public class HappyHourPromotion
    {
        public int Discount { get; set; }
        public int BeginHour { get; set; }
        public int EndHour { get; set; }
    }
}
