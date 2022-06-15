using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashDataService.Repositories
{
    public class UsedProgramDto : IEquatable<UsedProgramDto>
    {
        public DateTime DTime { get; set; }
        public int IdTerminal { get; set; }
        public string TerminalCode { get; set; }
        public string TerminalName { get; set; }
        public string ProgramCode { get; set; }
        public string ProgramName { get; set; }
        public int DisplayOrder { get; set; }
        public int Value { get; set; }

        public bool Equals([AllowNull] UsedProgramDto other)
        {
            if (other == null)
                return false;

            return this.ProgramCode == other.ProgramCode && this.Value == other.Value && this.IdTerminal == other.IdTerminal;
        }

        public override string ToString()
        {
            return $"{this.ProgramName}: {this.Value.ToString("#,0")}";
        }
    }
}
