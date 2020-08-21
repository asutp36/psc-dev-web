using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CardsMobileService.Controllers.Supplies
{
    public class GetBalanceFromMobile
    {
        [Required]
        public string dtime { get; set; }
        [Required]
        public string hash { get; set; }
        [Required]
        public string cardNum { get; set; }
    }
}
