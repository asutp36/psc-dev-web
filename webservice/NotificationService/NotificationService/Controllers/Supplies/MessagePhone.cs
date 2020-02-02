using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotificationService.Controllers.Supplies
{
    public class MessagePhone
    {
        public string phone { get; set; }
        public string body { get; set; }

        public MessagePhone(string num, string text)
        {
            this.phone = num;
            this.body = text;
        }
    }
}