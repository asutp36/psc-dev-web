using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashDataService.Models.Filters
{
    public class CardFilters
    {
        public IEnumerable<TerminalModel> Terminals { get; set; }
    }
}
