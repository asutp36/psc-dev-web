using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers.Supplies.ViewModels
{
    public class PostDiscountViewModel
    {
        public string Post { get; set; }
        public DateTime HappyHourBeg { get; set; }
        public DateTime HappyHourEnd { get; set; }
        public int HappyHourSale { get; set; }
    }
}
