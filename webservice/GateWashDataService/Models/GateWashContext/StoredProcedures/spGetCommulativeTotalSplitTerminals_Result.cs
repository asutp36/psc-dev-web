using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashDataService.Models.GateWashContext.StoredProcedures
{
    public class spGetCommulativeTotalSplitTerminals_Result
    {
        public DateTime DTimeBegin { get; set; }
        public string Code { get; set; }
        public string TerminalName { get; set; }
        public string ProgramName { get; set; }
        public int Amount { get; set; }
        public string IncreaseKind { get; set; }
        public int Total { get; set; }
    }
}
