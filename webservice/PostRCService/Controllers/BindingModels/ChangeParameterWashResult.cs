using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostRCService.Controllers.BindingModels
{
    public class ChangeParameterWashResult
    {
        public string wash { get; set; }
        public List<ChangeParameterResult> posts { get; set; }
    }
}
