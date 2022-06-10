using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashDataService.Models
{
    public class GroupedIncreaseModel
    {
        public DateTime DTime { get; set; }
        public string TerminalCode { get; set; }
        public string Terminal { get; set; }
        public string ProgramsDescription { get; set; }
        public string TypesDescription { get; set; }
        public int Amount { get; set; }
        public int ProgramCount { get; set; }
        public decimal ARPU
        {
            get
            {
                return this.Amount / this.ProgramCount;
            }
            private set { }
        }
    }
}
