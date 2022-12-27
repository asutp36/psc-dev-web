using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoyalityService.Models
{
    public class Discount
    {
        public int Percent { get; set; }
        public int Ruble { get; set; }
        public string Programs { get; set; }
    }
}
