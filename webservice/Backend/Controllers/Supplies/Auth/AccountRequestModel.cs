using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers.Supplies.Auth
{
    public class AccountRequestModel
    {
        [Required]
        public string login { get; set; }
        [Required]
        public string password { get; set; }
        [Required]
        public string name { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        [Required]
        public string role { get; set; }
        [Required]
        public List<string> washes { get; set; }
    }
}
