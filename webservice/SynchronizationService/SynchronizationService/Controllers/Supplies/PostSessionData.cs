using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SynchronizationService.Controllers.Supplies
{
    public class PostSessionData
    {
        public string postCode { get; set; }
        public int idSessionOnPost { get; set; }
        public DateTime startDTime { get; set; }
        public string qr { get; set; }
        public string fiscalError { get; set; }
        public DateTime stopDTime { get; set; }
        public int amountCash { get; set; }
        public int amountBank { get; set; }
    }
}