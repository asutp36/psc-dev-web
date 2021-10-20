using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostRCService.Controllers.BindingModels
{
    public class WashParameter<T>
    {
        public string washCode { get; set; }
        public List<PostParameter<T>> posts { get; set; }
    }
}
