using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SynchronizationService.Controllers.Supplies
{
    public class OperationFromRequest
    {
        public string device { get; set; }
        public string operationType { get; set; }
        public string cardNum { get; set; }
        public DateTime dtime { get; set; }
        public int amount { get; set; }
        public int balance { get; set; }
        public int localizedID { get; set; }
    }
}