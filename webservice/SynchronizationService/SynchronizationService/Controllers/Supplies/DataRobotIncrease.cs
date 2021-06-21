using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SynchronizationService.Controllers.Supplies
{
    public class DataRobotIncrease
    {
        public string DeviceCode { get; set; }
        public int IDEventPost { get; set; }
        public int IDSessionPost { get; set; }
        public DateTime DTime { get; set; }
        public string EventKindCode { get; set; }
        public int amount { get; set; }
        public int m10 { get; set; }
        public int b50 { get; set; }
        public int b100 { get; set; }
        public int b200 { get; set; }
        public int b500 { get; set; }
        public int b1000 { get; set; }
        public int b2000 { get; set; }
    }
}