using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers.Supplies.Stored_Procedures
{
    public class GetDataEventsByChanger_Result
    {
        public int IDEventChanger { get; set; }
        public string ChangerCode { get; set; }
        public DateTime DTimeEvent { get; set; }
        public string KindEventCode { get; set; }
        public string KindEvent { get; set; }
        public string DataEvent { get; set; }
    }
}
