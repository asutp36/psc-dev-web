using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers.Supplies.Stored_Procedures
{
    public class GetIncreaseByEvents_Result
    {
        public Int16 RegionCode { get; set; }
        public string RegionName { get; set; }
        public string WashCode { get; set; }
        public string WashAddress { get; set; }
        public string PostCode { get; set; }
        public string PostName { get; set; }
        public DateTime DTime { get; set; }
        public int Amount { get; set; }
    }
}
