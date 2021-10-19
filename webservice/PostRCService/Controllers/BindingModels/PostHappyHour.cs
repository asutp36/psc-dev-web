using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostRCService.Controllers.BindingModels
{
    public class PostHappyHour
    {
        public string postCode { get; set; }
        public HappyHourModel value { get; set; }
    }
}
