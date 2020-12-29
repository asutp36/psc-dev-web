using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers.Supplies.ViewModels
{
    public class Device
    {
        public string deviceName { get; set; }
        public string deviceCode { get; set; }
        public string errlevel { get; set; }
        public string[] errors { get; set; }
    }
}
