using System;
using System.Collections.Generic;
using System.Text;

namespace WashPing.Models
{
    internal class PingCounter
    {
        public string Name { get; set; }
        public int FailedPingCount { get; set; }
    }
}
