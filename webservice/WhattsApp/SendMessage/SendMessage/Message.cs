using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMessage
{
    class Message
    {
        public string chatId { get; set; }
        public string body { get; set; }

        public Message(string chat, string text)
        {
            this.chatId = chat;
            this.body = text;
        }
    }
}
