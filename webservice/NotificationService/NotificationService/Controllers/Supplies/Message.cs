using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotificationService.Controllers
{
    public class MessageChatID
    {
        public string chatId { get; set; }
        public string body { get; set; }

        public MessageChatID(string chat, string text)
        {
            this.chatId = chat;
            this.body = text;
        }
    }
}