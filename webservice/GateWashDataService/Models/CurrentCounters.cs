using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashDataService.Models
{
    public class CurrentCounters
    {
        public DateTime DTime { get; set; }
        public Dictionary<string, int> Counters { get; set; }
    }
}
