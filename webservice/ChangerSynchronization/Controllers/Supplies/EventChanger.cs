﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChangerSynchronization.Controllers.Supplies
{
    public class EventChanger
    {
        [Required]
        public string changer { get; set; }

        [Required]
        public string eventKindCode { get; set; }

        [Required]
        public DateTime dtime { get; set; }

        public List<EventAcquiring> eventsAcquiring { get; set; }

        public List<EventWithNominals> eventsNominals { get; set; }

        public List<EventCard> eventsCard { get; set; }
    }
}
