using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers.Supplies
{
    public class WashViewModel
    {
        public int idWash { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public int idRegion { get; set; }
    }
}
