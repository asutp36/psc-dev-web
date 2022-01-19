using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostSyncService.Controllers.BindingModels
{
    public class EventBindingModel
    {
        public int idEventOnPost { get; set; }
        public string cardNum { get; set; }
        public string uuid { get; set; }
        public string deviceCode { get; set; }
        public string eventKindCode { get; set; }
        public string dtime { get; set; }
    }
}
