using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers.Supplies.ViewModels
{
    public class PostStateViewModel
    {
        public string postCode { get; set; }
        public string washCode { get; set; }
        public string lastPing { get; set; }
        public string networkState { get; set; }
        public int balance { get; set; }
        public string function { get; set; }
        public Increase lastIncrease { get; set; }
        public string lastSync { get; set; }
    }

    public class Increase
    {
        public string dtime { get; set; }
        public int amount { get; set; }
    }
}
