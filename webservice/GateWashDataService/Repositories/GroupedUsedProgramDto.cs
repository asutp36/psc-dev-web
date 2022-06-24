using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashDataService.Repositories
{
    public class GroupedUsedProgramDto : GroupedItemDto, IEquatable<GroupedUsedProgramDto>
    {
        public string ProgramCode { get; set; }
        public string ProgramName { get; set; }
        public int DisplayOrder { get; set; }

        public bool Equals([AllowNull] GroupedUsedProgramDto other)
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
