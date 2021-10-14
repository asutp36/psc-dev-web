using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers.Supplies.ViewModels
{
    public class RegionParameter<T>
    {
        public int regionCode { get; set; }
        public string regionName { get; set; }
        public List<WashParameter<T>> washes { get; set; }
    }
}
