using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PostControllingService.Controllers.Supplies
{
    public class PostDiscount
    {
        public string Post { get; set; }
        public DateTime HappyHourBeg { get; set; }
        public DateTime HappyHourEnd { get; set; }
        public int HappyHourSale { get; set; }
    }
}