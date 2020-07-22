using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChangerSynchronization.Controllers.Supplies
{
    public class EventWithNominals : Event
    {
        [Required]
        public int m10 { get; set; }
        [Required]
        public int b50 { get; set; }
        [Required]
        public int b100 { get; set; }
        public int b200 { get; set; }
        public int b500 { get; set; }
        public int b1000 { get; set; }
        public int b2000 { get; set; }
    }
}
