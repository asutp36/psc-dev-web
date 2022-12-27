using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoyalityService.Models
{
    public class StartPostParameters
    {
        public string DeviceCode { get; set; }
        public int DiscountPercent { get; set; }
        public int DiscountRub { get; set; }
        public long ClientPhone { get; set; }
        public string Programs { get; set; }
    }
}
