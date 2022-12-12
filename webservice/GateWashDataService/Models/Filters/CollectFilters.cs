using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashDataService.Models.Filters
{
    public class CollectFilters
    {
        public IEnumerable<TerminalModel> Terminals { get; set; }
    }
}
