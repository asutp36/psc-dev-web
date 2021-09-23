using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostRCService.Controllers.BindingModels
{
    public class ChangeHappyHourWash
    {
        public string[] washes { get; set; }
        public HappyHourModel happyHour { get; set; }
    }
}
