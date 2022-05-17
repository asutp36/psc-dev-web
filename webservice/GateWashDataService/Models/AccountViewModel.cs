using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashDataService.Models
{
    public class AccountViewModel
    {
        public string login { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string role { get; set; }
        public List<string> washes { get; set; }
    }
}
