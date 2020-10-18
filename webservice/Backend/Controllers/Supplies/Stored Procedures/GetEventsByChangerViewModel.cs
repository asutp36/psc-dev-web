using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers.Supplies.Stored_Procedures
{
    public class GetEventsByChangerViewModel
    {
        public string ChangerName { get; set; }
        public DateTime DTime { get; set; }
        public string KindEvent { get; set; }
        public List<GetDataEventsByChanger_Result> Details { get; set; }

        public GetEventsByChangerViewModel(string changer, DateTime dtime, string eventKind)
        {
            this.ChangerName = changer;
            this.DTime = dtime;
            this.KindEvent = eventKind;
            this.Details = new List<GetDataEventsByChanger_Result>();
        }
    }
}
