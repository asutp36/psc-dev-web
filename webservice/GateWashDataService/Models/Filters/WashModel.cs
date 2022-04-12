using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashDataService.Models.Filters
{
    public class WashModel
    {
        public int IdWash { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int IdRegion { get; set; }
    }
}
