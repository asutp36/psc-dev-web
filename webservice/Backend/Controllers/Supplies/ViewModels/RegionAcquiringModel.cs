using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers.Supplies.ViewModels
{
    public class RegionAcquiringModel
    {
        public int regionCode { get; set; }
        public string regionName { get; set; }
        public List<WashAcquiringViewModel> washes { get; set; }
    }
}
