using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscountService.Models
{
    public class StartPostParameters
    {
        public string DeviceCode { get; set; }
        public int Discount { get; set; }
    }
}
