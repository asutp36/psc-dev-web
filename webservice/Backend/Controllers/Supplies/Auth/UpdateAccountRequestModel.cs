using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers.Supplies.Auth
{
    public class UpdateAccountRequestModel : AccountRequestModel
    {
        [Required]
        public string oldLogin { get; set; }
    }
}
