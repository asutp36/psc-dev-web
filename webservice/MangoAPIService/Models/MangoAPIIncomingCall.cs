using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MangoAPIService.Models
{
    public class MangoAPIIncomingCall
    {
        public MangoAPIFromAbonent from { get; set; }
        public MangoAPIToAbonent to { get; set; }
    }
}
