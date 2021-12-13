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
        public double FiscalPercent { get; set; }

        public int sumofm { get { return this.m10 * 10; } }
        public int sumofb { get { return (this.b10 * 10) + (this.b50 * 50) + (this.b100 * 100) + (this.b200 * 200); } }
        public int sumall { get { return this.sumofm + this.sumofb; } }
    }
}

