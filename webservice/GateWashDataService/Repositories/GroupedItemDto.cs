using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashDataService.Repositories
{
    public abstract class GroupedItemDto
    {
        public DateTime DTime { get; set; }
        public int Hour { get; set; }
        public int IdTerminal { get; set; }
        public string TerminalCode { get; set; }
        public string TerminalName { get; set; }
        public double Value { get; set; }
    }
}
