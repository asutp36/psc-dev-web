using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotNotificationService.Models
{
    public class SendMessageWhattsAppModel
    {
        public string chatId { get; set; }
        public string body { get; set; }
    }
}
