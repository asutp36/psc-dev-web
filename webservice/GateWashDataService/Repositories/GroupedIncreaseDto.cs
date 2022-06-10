using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashDataService.Repositories
{
    public class GroupedIncreaseDto
    {
        public DateTime DTime { get; set; }
        public string TerminalCode { get; set; }
        public string Terminal { get; set; }
        public IEnumerable<UsedProgramDto> Programs { get; set; }
        public IEnumerable<IncreaseTypeDto> Types { get; set; }
    }
}
