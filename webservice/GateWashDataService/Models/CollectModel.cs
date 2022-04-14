using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashDataService.Models
{
    public class CollectModel
    {
        public DateTime DTime { get; set; }
        public string Terminal { get; set; }
        public int m10 { get; set; }
        public int b50 { get; set; }
        public int b100 { get; set; }
        public int b200 { get; set; }
        public int b500 { get; set; }
        public int b1000 { get; set; }
        public int b2000 { get; set; }
        public int Sum { get
            {
                return m10 * 10 + b50 * 50 + b100 * 100 + b200 * 200 + b500 * 500 + b1000 * 1000 + b2000 * 2000;
            }
        }
    }
}
