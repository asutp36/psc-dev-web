using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashDataService.Models
{
    public class TerminalWithActions
    {
        public int IdTerminal { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public TerminalType Type { get; set; }
        public string InsertAction { get; set; }
    }
}
