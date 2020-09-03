using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers.Supplies.Auth
{
    public class LoginModel
    {
        [Required]
        public string login { get; set; }
        [Required]
        public string password { get; set; }
    }
}
