using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashDataService.Models
{
    public class TerminalCardsInsertionModel
    {
        public string TerminalCode { get; set; }
        public int RightDispenser { get; set; }
        public int LeftDispenser { get; set; }
    }
}
