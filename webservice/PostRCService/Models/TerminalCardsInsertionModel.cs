using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostRCService.Models
{
    public class TerminalCardsInsertionModel
    {
        public string terminalCode { get; set; }
        public int cards1 { get; set; }
        public int cards2 { get; set; }
        public string login { get; set; }
    }
}
