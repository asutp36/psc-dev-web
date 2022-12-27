using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace PostRCService.Controllers.BindingModels
{
    public class PostStartingDiscountParameters
    {
        public int discountPercent { get; set; }
        public int discountRub { get; set; }
        public long clientPhone { get; set; }
        public string programs { get; set; }
    }
}
