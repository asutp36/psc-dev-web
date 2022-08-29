using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoyalityService.Models
{
    public class ClientEachNWashStatus
    {
        public int Discount { get; set; }
        public int DiscountRub { get; set; }
        public int CurrentWashCount { get; set; }
        public int N { get; set; }
    }
}
