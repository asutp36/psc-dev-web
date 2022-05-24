using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotNotificationService.Models
{
    public class SendMessage
    {
        public string chat_id { get; set; }
        public string text { get; set; }
    }
}
