﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebService.Controllers.Supplies
{
    public class Message
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