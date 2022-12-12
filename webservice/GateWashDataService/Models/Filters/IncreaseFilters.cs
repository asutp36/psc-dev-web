using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashDataService.Models.Filters
{
    public class IncreaseFilters
    {
        public IEnumerable<TerminalModel> Terminals { get; set; }
        public IEnumerable<ProgramModel> Programs { get; set; }
        public IEnumerable<EventKindModel> Types { get; set; }
    }
}
