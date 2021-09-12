using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers.Supplies.ViewModels
{
    public class RateWPostCode
    {
        public string Post { get; set; }
        public List<RateViewModel> Prices { get; set; }
    }
}
