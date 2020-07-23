using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ChangerSynchronization_framework.Controllers.Supplies
{
    public class EventAcquiring : Event
    {
        [Required]
        public int amount { get; set; }
    }
}