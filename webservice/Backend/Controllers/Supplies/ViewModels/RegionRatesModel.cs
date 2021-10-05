using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers.Supplies.ViewModels
{
    public class RegionRatesModel
    {
        public int regionCode { get; set; }
        public string regionName { get; set; }
        public List<WashRatesViewModel> washes { get; set; }
    }
}
