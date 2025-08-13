using System.Collections.Generic;

namespace BotNotificationService.Options
{
    public class TelegramBotOptions
    {
        public string Token { get; set; }
        public List<long> AcceptableCommandUsers { get; set; }
    }
}
