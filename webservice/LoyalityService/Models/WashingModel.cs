using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoyalityService.Models
{
    public class WashingModel
    {
        public long ClientPhone { get; set; }
        public string DTime { get; set; }
        public string Device { get; set; }
        public string Program { get; set; }
        public int Amount { get; set; }
        public int Discount { get; set; }
        public string Guid { get; set; }
        public int DiscountRub { get; set; }
    }
}
