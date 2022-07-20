using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoyalityService.Models
{
    public class IncomeCallModel
    {
        public string From { get; set; }
        public string To { get; set; }
        public DateTime When { get; set; }
    }
}
