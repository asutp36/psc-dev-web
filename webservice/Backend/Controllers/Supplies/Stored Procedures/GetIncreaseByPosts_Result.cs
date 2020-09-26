using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers.Supplies.Stored_Procedures
{
    public class GetIncreaseByPosts_Result
    {
        public Int16 RegionCode { get; set; }
        public string RegionName { get; set; }
        public string WashCode { get; set; }
        public string WashAddress { get; set; }
        public string PostCode { get; set; }
        public string PostName { get; set; }
        public int m10 { get; set; }
        public int b10 { get; set; }
        public int b50 { get; set; }
        public int b100 { get; set; }
        public int b200 { get; set; }
    }
}

