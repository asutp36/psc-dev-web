using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashDataService.Models
{
    public class RegionWithWashTerminalActions
    {
        public int IdRegion { get; set; }
        public int Code { get; set; }
        public string Name { get; set; }
        public IEnumerable<WashWithTerminalsActions> Washes { get; set; }
    }
}
