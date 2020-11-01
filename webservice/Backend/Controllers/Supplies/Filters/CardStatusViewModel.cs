using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers.Supplies.Filters
{
    public class CardStatusViewModel
    {
        [Required]
        public string code { get; set; }
        [Required]
        public string name { get; set; }
    }
}
