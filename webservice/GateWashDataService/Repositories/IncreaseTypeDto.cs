using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashDataService.Repositories
{
    public class IncreaseTypeDto : IEquatable<IncreaseTypeDto>
    {
        public DateTime DTime { get; set; }
        public int IdTerminal { get; set; }
        public string TerminalCode { get; set; }
        public string TerminalName { get; set; }
        public string TypeCode { get; set; }
        public string TypeName { get; set; }
        public int DisplayOrder { get; set; }
        public int Value { get; set; }

        public bool Equals([AllowNull] IncreaseTypeDto other)
        {
            if (other == null)
                return false;

            return this.TypeCode == other.TypeCode && this.Value == other.Value && this.IdTerminal == other.IdTerminal;
        }

        public override string ToString()
        {
            return $"{this.TypeName}: {this.Value.ToString("#,0")}";
        }
    }
}
