using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers.Supplies.ViewModels
{
    public class TechCardModel
    {
        [Required]
        public string num { get; set; }
        [Required]
        public string typeCode { get; set; }
        [Required]
        public List<string> groupCodes { get; set; }
    }
}
