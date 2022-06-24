using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashDataService.Repositories
{
    public class GroupedIncreaseTypeDto : GroupedItemDto, IEquatable<GroupedIncreaseTypeDto>
    {
        public string TypeCode { get; set; }
        public string TypeName { get; set; }
        public int DisplayOrder { get; set; }

        public bool Equals([AllowNull] GroupedIncreaseTypeDto other)
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
