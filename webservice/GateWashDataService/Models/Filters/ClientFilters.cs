using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashDataService.Models.Filters
{
    public class ClientFilters
    {
        public IEnumerable<TerminalModel> EnterTerminals { get; set; }
        public IEnumerable<TerminalModel> ExitTerminals { get; set; }
        public IEnumerable<ProgramModel> Programs { get; set; }
    }
}
