using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashDataService.Models
{
    public class WashWithTerminalsActions
    {
        public int IdWash { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public IEnumerable<TerminalWithActions> Terminals { get; set; }
    }
}
