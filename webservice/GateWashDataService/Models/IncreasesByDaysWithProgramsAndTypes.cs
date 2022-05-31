using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashDataService.Models
{
    public class IncreasesByDaysWithProgramsAndTypes
    {
        public DateTime DTime { get; set; }
        public string TerminalCode { get; set; }
        public string Terminal { get; set; }
        public int Amount
        {
            get
            {
                int amount = 0;
                if (this.Types != null)
                {
                    foreach (IncrType t in this.Types)
                        amount += t.Value;
                }

                return amount;
            }
            private set { }
        }
        public List<IncrProgram> Programs { get; set; }
        public List<IncrType> Types { get; set; }
        public int ProgramCount
        {
            get
            {
                if (this.Programs != null)
                    return this.Programs.Sum(v => v.Value);
                return 0;

            }
            private set { }
        }

        public decimal ARPU
        {
            get
            {
                if (this.ProgramCount != 0)
                    return this.Amount / this.ProgramCount;
                return 0;
            }
            private set { }
        }
    }
}
