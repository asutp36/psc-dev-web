using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoyalityService.Models
{
    public class HolidayPromotion
    {
        public int Discount { get; set; }
        public int DiscountRub { get; set; }
        public DateTime Date { get; set; }
    }
}
