using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashDataService.Models
{
    public class ClientExit : ClientEntrance
    {
        public string PayTerminal { get; set; }
        public string PayTerminalCode { get; set; }
        public string PayType { get; set; }
    }
}
