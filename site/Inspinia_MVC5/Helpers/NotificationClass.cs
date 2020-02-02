using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inspinia_MVC5.Helpers
{
    public class NotificationClass
    {
        public string sender { get; set; }
        public bool isPhone { get; set; }
        public string receiver { get; set; }
        public string body { get; set; }

        public NotificationClass(string sender, bool isPhone, string receiver, string theme, string text)
        {
            if (theme != "")
            {
                theme = theme + "\n";
            }

            string body = theme.ToUpper() + text;

            this.sender = sender;
            this.isPhone = isPhone;
            this.receiver = receiver;
            this.body = body;
        }
    }
}