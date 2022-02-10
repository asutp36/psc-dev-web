using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashSyncService.Controllers.BindingModels
{
    public class SessionBindingModel
    {
        public int idSession { get; set; }
        public string functionCode { get; set; }
        public string cardNum { get; set; }
        public string dtime { get; set; }
        public string uuid { get; set; }
    }
}
