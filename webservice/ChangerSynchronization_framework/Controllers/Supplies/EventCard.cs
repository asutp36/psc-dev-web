using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ChangerSynchronization_framework.Controllers.Supplies
{
    public class EventCard : Event
    {
        [Required]
        public string cardNum { get; set; }
        public string phone { get; set; }
    }
}