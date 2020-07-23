using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ChangerSynchronization_framework.Controllers.Supplies
{
    public class EventWithNominals : Event
    {
        [Required]
        public string eventKind { get; set; } // increase or out
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