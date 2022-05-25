using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotNotificationService.Models
{
    public class BotResponse
    {
        public bool ok { get; set; }
        public int error_code { get; set; }
        public string description { get; set; }
    }
}
