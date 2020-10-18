using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers.Supplies.Stored_Procedures
{
    public class GetSumsByChanger_Result
    {
        public int IDChanger { get; set; }
        public string ChangerCode { get; set; }
        public string IpAddress { get; set; }
        public int sincrease { get; set; }
        public int ccard { internal get; set; }
        public int sout { internal get; set; }
    }
}
