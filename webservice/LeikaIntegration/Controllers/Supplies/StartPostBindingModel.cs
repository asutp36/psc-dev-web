using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;

namespace LeikaIntegration.Controllers.Supplies
{
    public class StartPostBindingModel
    {
        public string dtime { get; set; }
        public string hash { get; set; }
        public string post { get; set; }
        public int amount { get; set; }
    }
}
