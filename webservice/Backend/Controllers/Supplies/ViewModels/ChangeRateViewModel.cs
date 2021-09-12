using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers.Supplies.ViewModels
{
    public class ChangeRateViewModel
    {
        public List<string> Washes { get; set; }
        public List<RateViewModel> Rates { get; set; }
    }
}
