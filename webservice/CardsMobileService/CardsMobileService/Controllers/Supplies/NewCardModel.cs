using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CardsMobileService.Controllers.Supplies
{
    public class NewCardModel
    {
        [Required]
        public string dtime { get; set; }
        [Required]
        public string cardNum { get; set; }
        [Required]
        public string phone { get; set; }
    }
}
