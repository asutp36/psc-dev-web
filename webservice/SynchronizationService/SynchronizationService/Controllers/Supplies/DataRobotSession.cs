using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SynchronizationService.Controllers.Supplies
{
    public class DataRobotSession
    {
        public string DeviceCode { get; set; }
        public DateTime DTime { get; set; }
        public string ProgramCode { get; set; }
        public int IDSessionPost { get; set; }
    }
}