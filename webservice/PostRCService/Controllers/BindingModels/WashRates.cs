using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostRCService.Controllers.BindingModels
{
    public class WashRates
    {
        public string wash { get; set; }
        public List<PostRates> rates { get; set; }
    }
}
