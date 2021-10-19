using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostRCService.Controllers.BindingModels
{
    public class WashHappyHour
    {
        public string washCode { get; set; }
        public List<PostHappyHour> posts { get; set; }
    }
}
