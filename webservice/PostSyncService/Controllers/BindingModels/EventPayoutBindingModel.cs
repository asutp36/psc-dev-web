using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostSyncService.Controllers.BindingModels
{
    public class EventPayoutBindingModel
    {
        public string deviceCode { get; set; }
        public int idEventOnPost { get; set; }
        public string cardNum { get; set; }
        public string uuid { get; set; }
        public string dtime { get; set; }
        public string eventKindCode { get; set; }
        public int amount { get; set; }
        public int b50 { get; set; }
        public int b100 { get; set; }
        public int storage_b50 { get; set; }
        public int storage_b100 { get; set; }
    }
}
