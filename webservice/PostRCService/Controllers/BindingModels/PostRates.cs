using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostRCService.Controllers.BindingModels
{
    public class PostRates
    {
        public string postCode { get; set; }
        public RatesModel value { get; set; }
    }
}
