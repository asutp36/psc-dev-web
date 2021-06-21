using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SynchronizationService.Controllers.Supplies
{
    public class DataRobotPayout
    {
        public string DeviceCode { get; set; }
        public int IDEventPost { get; set; }
        public int IDSessionPost { get; set; }
        public DateTime DTime { get; set; }
        public string EventKindCode { get; set; }
        public int amount { get; set; }
        public int b50 { get; set; }
        public int b100 { get; set; }
        public int storage_b50 { get; set; }
        public int storage_b100 { get; set; }
    }
}