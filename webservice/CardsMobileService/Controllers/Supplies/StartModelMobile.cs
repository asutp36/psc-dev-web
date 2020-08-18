using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CardsMobileService.Controllers.Supplies
{
    public class StartModelMobile : PostActionModel
    {
        [Required]
        public string hash { get; set; }
        [Required]
        public string dtime { get; set; }
    }
}
