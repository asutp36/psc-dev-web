using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HangFireTest.JobHelpers.WhattAppReportSender
{
    public class MessageToSend
    {
        public string chatId { get; set; }
        public string body { get; set; }
    }
}
