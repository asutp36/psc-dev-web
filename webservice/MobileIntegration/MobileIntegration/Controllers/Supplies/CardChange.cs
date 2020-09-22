using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MobileIntegration.Controllers.Supplies
{
    public class CardChange
    {
        [Required]
        public string oldNum { get; set; }
        [Required]
        public string newNum { get; set; }
    }
}