using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashDataService.Models
{
    public class CardIssuance
    {
        public string Terminal { get; set; }
        public DateTime DTime { get; set; }
        public int Dispenser1 { get; set; }
        public int Dispenser2 { get; set; }
        public int Count1 { get; set; }
        public int Count2 { get; set; }
    }
}
