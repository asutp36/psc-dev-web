using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MangoAPIService.Models
{
    public class MangoAPIIncomingCall
    {
        public string call_id { get; set; }
        public MangoAPIFromAbonent from { get; set; }
        public MangoAPIToAbonent to { get; set; }
    }
}
