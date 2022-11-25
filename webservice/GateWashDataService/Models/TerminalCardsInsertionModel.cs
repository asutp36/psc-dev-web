using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashDataService.Models
{
    public class TerminalCardsInsertionModel
    {
        public string TerminalCode { get; set; }
        public int Cards1 { get; set; }
        public int Cards2 { get; set; }
        public string Login { get; set; }
    }
}
