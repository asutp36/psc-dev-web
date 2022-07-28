using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoyalityService.Models
{
    public class EachNWashPromotion : Promotion
    {
        public int EachN { get; set; }
        public int Days { get; set; }
    }
}
