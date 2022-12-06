using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashSyncService.Controllers.BindingModels
{
    public class CardCounterModel
    {
        public int idCardOperation { get; set; }
        public string deviceCode { get; set; }
        public string eventKindCode { get; set; }
        public string dtime { get; set; }
        public string login { get; set; }
        public int dispenser1 { get; set; }
        public int dispenser2 { get; set; }
        public int count1 { get; set; }
        public int count2 { get; set; }
    }
}
