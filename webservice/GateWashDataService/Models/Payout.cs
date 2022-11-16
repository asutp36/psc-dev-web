using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashDataService.Models
{
    public class Payout
    {
        public string Terminal { get; set; }
        public DateTime DTime { get; set; }
        public int Amount { get; set; }
        public int M10 { get; set; }
        public int B50 { get; set; }
        public int B100 { get; set; }
    }
}
