using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PostControllingService.Controllers.Supplies
{
    public class PostDiscount
    {
        [Required]
        public string Post { get; set; }
        [Required]
        public DateTime HappyHourBeg { get; set; }
        [Required]
        public DateTime HappyHourEnd { get; set; }
        [Required]
        public int HappyHourSale { get; set; }
    }
}