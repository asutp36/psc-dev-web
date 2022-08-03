using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostRCService.Controllers.BindingModels
{
    public class PostStartingDiscountParameters
    {
        public int discount { get; set; }
        public long clientPhone { get; set; }
    }
}
