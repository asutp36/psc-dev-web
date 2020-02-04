using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotificationService.Controllers.Supplies
{
    public class MessageToSend
    {
        public string sender { get; set; }
        public string receiver { get; set; }
        public string body { get; set; }
    }
}