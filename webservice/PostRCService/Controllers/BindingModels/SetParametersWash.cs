using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostRCService.Controllers.BindingModels
{
    public class SetParametersWash<T>
    {
        public string washCode { get; set; }
        public T value { get; set; }
    }
}
