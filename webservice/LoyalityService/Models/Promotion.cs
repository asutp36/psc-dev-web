using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoyalityService.Models
{
    public abstract class Promotion
    {
        public int Discount { get; set; }
        public string Group { get; set; }
        public string Type { get; set; }
        public int ApplyPriority { get; set; }
    }
}
