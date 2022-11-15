using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashSyncService.Controllers.BindingModels
{
    public class EventPayoutBindingModel
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
        public int inbox_1_b50 { get; set; }
        public int inbox_2_b50 { get; set; }
        public int inbox_3_b100 { get; set; }
        public int inbox_4_b100 { get; set; }
        public int inbox_5_m10 { get; set; }
    }
}
