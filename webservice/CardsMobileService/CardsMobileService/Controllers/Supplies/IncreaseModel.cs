using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CardsMobileService.Controllers.Supplies
{
    public class IncreaseModel
    {
        [Required]
        public string dtime { get; set; }
        [Required]
        public string cardNum { get; set; }
        [Required]
        public int amount { get; set; }
        [Required]
        public string operationType { get; set; }
    }
}
