using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;

namespace LeikaIntegration.Controllers.Supplies
{
    public class StartPostRequestBindingModel
    {
        [Required]
        public string dtime { get; set; }
        [Required]
        public string hash { get; set; }
        [Required]
        public string post { get; set; }
        [Required]
        public int amount { get; set; }
        public string clientID { get; set; }
    }
}
