using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashDataService.Models
{
    public class IncreaseCommulativeTotalModel
    {
        public DateTime DTime { get; set; }
        public string Terminal { get; set; }
        public string TerminalCode { get; set; }
        public int Amount { get; set; }
    }
}
