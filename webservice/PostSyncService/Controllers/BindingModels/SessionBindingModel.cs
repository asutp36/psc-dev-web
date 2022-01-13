using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostSyncService.Controllers.BindingModels
{
    public class SessionBindingModel
    {
        public int idSession { get; set; }
        public string deviceCode { get; set; }
        public string functionCode { get; set; }
        public string idCard { get; set; }
        public string dtime { get; set; }
        public string uuid { get; set; }
    }
}
