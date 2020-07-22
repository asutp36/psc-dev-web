using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChangerSynchronization.Controllers.Supplies
{
    public class Event
    {
        [Required]
        public DateTime dtime { get; set; }
    }
}
