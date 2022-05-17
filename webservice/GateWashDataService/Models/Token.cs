using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashDataService.Models
{
    public class Token
    {
        [Required]
        public string accessToken { get; set; }
        [Required]
        public string login { get; set; }
        [Required]
        public string role { get; set; }
        [Required]
        public string name { get; set; }
    }
}
