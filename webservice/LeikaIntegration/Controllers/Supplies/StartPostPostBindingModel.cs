using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LeikaIntegration.Controllers.Supplies
{
    public class StartPostPostBindingModel
    {
        [Required]
        public int Amount { get; set; }
        public string ClientId { get; set; }
    }
}
