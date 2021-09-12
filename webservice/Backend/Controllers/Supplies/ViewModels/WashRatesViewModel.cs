using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers.Supplies.ViewModels
{
    public class WashRatesViewModel
    {
        public string Wash { get; set; }
        public List<RateWPostCode> Rates { get; set; }
    }
}
