using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers.Supplies.Stored_Procedures
{
    public class GetEventsByChanger_Result
    {
        public string ChangerCode { get; set; }
        public string ChangerName { get; set; }
        public int IDEventChanger { get; set; }
        public DateTime DTime { get; set; }
        public string KindEventCode { get; set; }
        public string KindEvent { get; set; }
    }
}
