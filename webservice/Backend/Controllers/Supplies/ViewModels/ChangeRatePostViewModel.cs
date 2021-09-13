using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers.Supplies.ViewModels
{
    public class ChangeRatePostViewModel
    {
        public string PostCode { get; set; }
        public List<RateViewModel> Rates { get; set; }
    }
}
