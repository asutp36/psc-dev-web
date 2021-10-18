using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostRCService.Controllers.BindingModels
{
    public class ChangeRatesWash
    {
        public string washCode { get; set; }
        public RatesModel value { get; set; }
    }
}
