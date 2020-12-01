using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers.Supplies.Auth
{
    public class AccountViewModel
    {
        public string login { get; set; }
        public string email { get; set; }
        public string description { get; set; }
        public string role { get; set; }
        public List<string> washes { get; set; }
    }
}
