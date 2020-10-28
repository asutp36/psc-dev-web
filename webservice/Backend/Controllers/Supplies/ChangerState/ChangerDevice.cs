using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers.Supplies.ChangerState
{
    public class ChangerDevice
    {
        public string devicename { get; set; }
        public string devicecode { get; set; }
        public string errlevel { get; set; }
        public string[] errors { get; set; }
    }
}
