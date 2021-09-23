using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostRCService.Controllers.BindingModels
{
    public class PostAcquiring
    {
        public string post { get; set; }
        public AcquiringModel acquiring { get; set; }
    }
}
