using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashDataService.Models
{
    public class UpdateAccountRequestModel
    {
        [Required]
        public string oldLogin { get; set; }
        [Required]
        public string login { get; set; }
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
