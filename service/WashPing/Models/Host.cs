using System;
using System.Collections.Generic;
using System.Text;

namespace WashPing.Models
{
    class Host
    {
        public string Name { get; set; }
        public string Ip { get; set; }
        public string FailedChatID { get; set; }
        public string FailedMessage { get; set; }
    }
}
