using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashSyncService.Controllers.BindingModels
{
    public class PaySessionBindingModel
    {
        public int idSessionOnPost { get; set; }
        public string dtimeBegin { get; set; }
        public string functionCode { get; set; }
        public string deviceCode { get; set; }
        public int programCost { get; set; }
        public string qr { get; set; }
        public string fiscalError { get; set; }
        public string dtimeEnd { get; set; }
        public string details { get; set; }
        public string uuid { get; set; }
    }
}
