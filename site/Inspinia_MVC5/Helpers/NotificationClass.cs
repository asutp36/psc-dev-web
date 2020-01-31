using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inspinia_MVC5.Helpers
{
    public class NotificationClass
    {
        public string Receiver { get; set;}

        public string Message { get; set; }

        public NotificationClass(string Receiver, string Theme, string Text)
        {
            if (Theme == null)
            {
                Theme = "";
            }
            else
            {
                Theme = Theme + "\n";
            }

            string Message = Theme.ToUpper() + Text;

            this.Receiver = Receiver;
            this.Message = Message;
        }
    }
}