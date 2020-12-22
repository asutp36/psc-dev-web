using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers.Supplies.ViewModels
{
    public class PostStatisticViewModel
    {
        public string postCode { get; set; }
        public string washCode { get; set; }
        public string lastPing { get; set; }
        public int sumIncrease { get; set; }
        public int sumall { get; set; }
        public int sumofb { get; set; }
        public int sumofm { get; set; }
        public int washMins { get; set; }
    }
}
