using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PostControllingService.Controllers.Supplies
{
    public class PostAcquiring
    {
        [Required]
        public string Post { get; set; }
        [Required]
        public int BankAmountMin { get; set; }
        [Required]
        public int BankAmountMax { get; set; }
        [Required]
        public int BankAmountStep { get; set; }
    }
}