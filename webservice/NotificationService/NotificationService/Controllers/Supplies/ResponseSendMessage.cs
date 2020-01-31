using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotificationService.Controllers.Supplies
{
    public class ResponseSendMessage
    {
        public bool sent { get; set; }
        public string message { get; set; }
    }
}