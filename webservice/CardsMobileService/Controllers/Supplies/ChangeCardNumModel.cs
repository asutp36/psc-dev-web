using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CardsMobileService.Controllers.Supplies
{
    public class ChangeCardNumModel
    {
        [Required]
        public string changer { get; set; }
        [Required]
        public string oldNum { get; set; }
        [Required]
        public string newNum { get; set; }
    }
}
