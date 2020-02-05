using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inspinia_MVC5.Helpers
{
    public class NotificationClass
    {
        public string sender { get; set; }
        public string receiver { get; set; }
        public string body { get; set; }

        public NotificationClass(string sender, string receiver, string body)
        {
            this.sender = sender;
            this.receiver = receiver;
            this.body = body;
        }
    }
}