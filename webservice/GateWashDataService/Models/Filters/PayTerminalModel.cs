using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashDataService.Models.Filters
{
    public class PayTerminalModel
    {
        public int IdDevice { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int IdWash { get; set; }
    }
}
