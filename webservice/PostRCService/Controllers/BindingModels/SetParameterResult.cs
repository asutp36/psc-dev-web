using PostRCService.Controllers.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostRCService.Controllers.BindingModels
{
    public class SetParameterResult
    {
        public string post { get; set; }
        public HttpResponse result { get; set; }
    }
}
