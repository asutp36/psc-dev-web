using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostRCService.Controllers.BindingModels
{
    public class SetParameterWashResult
    {
        public string wash { get; set; }
        public List<SetParameterResult> posts { get; set; }
    }
}
