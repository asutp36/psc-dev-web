using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotNotificationService.Models
{
    public class Update
    {
        public int update_id { get; set; }
        public Message message { get; set; }
        public Message channel_post { get; set; }

    }
}
