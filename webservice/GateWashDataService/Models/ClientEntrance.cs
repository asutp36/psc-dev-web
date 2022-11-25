using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashDataService.Models
{
    public class ClientEntrance
    {
        public string Terminal { get; set; }
        public string TerminalCode { get; set; }
        public DateTime DTime { get; set; }
        public string Card { get; set; }
        public string Program { get; set; }
        public int Cost { get; set; }
    }
}
