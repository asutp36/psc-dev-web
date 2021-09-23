using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers.Supplies.ViewModels
{
    public class RateWPostCode
    {
        public string post { get; set; }
        public List<RateViewModel> prices { get; set; }
    }
}
