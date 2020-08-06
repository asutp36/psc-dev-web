using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CardsMobileService.Controllers.Supplies
{
    public class IncreaseFromMobile : IncreaseModel
    {
        [Required]
        public string hash { get; set; }
    }
}
