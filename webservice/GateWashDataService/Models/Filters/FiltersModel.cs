using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashDataService.Models.Filters
{
    public class FiltersModel
    {
        public List<RegionModel> Regions { get; set; }
        public List<WashModel> Washes { get; set; }
        public List<PayTerminalModel> PayTerminals { get; set; }
        public List<ProgramModel> Programs { get; set; }
        public List<EventKindModel> EventKinds { get; set; }
    }
}
