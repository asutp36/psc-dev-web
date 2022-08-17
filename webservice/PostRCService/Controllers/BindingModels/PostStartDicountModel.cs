using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostRCService.Controllers.BindingModels
{
    public class PostStartDicountModel
    {
        public string DeviceCode { get; set; }
        public int DiscountPercent { get; set; }
        public int DiscountRub { get; set; }
        public long ClientPhone { get; set; }
    }
}
