using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotNotificationService.Models
{
    public class Message
    {
        public string text { get; set; }
        public Chat chat { get; set; }
        public User from { get; set; }
        public bool group_chat_created { get; set; }
    }
}
