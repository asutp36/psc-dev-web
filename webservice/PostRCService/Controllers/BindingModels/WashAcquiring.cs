using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostRCService.Controllers.BindingModels
{
    public class WashAcquiring
    {
        public string wash { get; set; }
        public List<PostAcquiring> posts { get; set; }
    }
}
