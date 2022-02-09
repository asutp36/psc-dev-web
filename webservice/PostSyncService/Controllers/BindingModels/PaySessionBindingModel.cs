using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostSyncService.Controllers.BindingModels
{
    public class PaySessionBindingModel
    {
        public int idSessionOnPost { get; set; }
        public string dtime { get; set; }
        public string functionCode { get; set; }
        public string deviceCode { get; set; }
    }
}
