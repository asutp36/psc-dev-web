using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashSyncService.Controllers.BindingModels
{
    public class EventCollectBindingModel
    {
        public string deviceCode { get; set; }
        public int idSessionOnPost { get; set; }
        public int idEventOnPost { get; set; }
        public string dtime { get; set; }
        public string eventKindCode { get; set; }
        public int amount { get; set; }
        public int m10 { get; set; }
        public int b50 { get; set; }
        public int b100 { get; set; }
        public int b200 { get; set; }
        public int b500 { get; set; }
        public int b1000 { get; set; }
        public int b2000 { get; set; }
        public int box_b50 { get; set; }
        public int box_b100 { get; set; }
        public int inbox_b50 { get; set; }
        public int inbox_b100 { get; set; }
    }
}
