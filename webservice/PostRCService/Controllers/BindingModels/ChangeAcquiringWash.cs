using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostRCService.Controllers.BindingModels
{
    public class ChangeAcquiringWash
    {
        public string[] washes { get; set; }
        public AcquiringModel acquiring { get; set; }
    }
}
