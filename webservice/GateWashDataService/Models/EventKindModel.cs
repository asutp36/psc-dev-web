using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashDataService.Models
{
    public class EventKindModel
    {
        public int IdEventKind { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public List<int> IdWash { get; set; }
    }
}
