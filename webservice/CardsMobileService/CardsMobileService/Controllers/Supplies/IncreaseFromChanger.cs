using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CardsMobileService.Controllers.Supplies
{
    public class IncreaseFromChanger : IncreaseModel
    {
        [Required]
        public string changer { get; set; }
        [Required]
        public int localizedID { get; set; }
    }
}
