using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Inspinia_MVC5.Models;

namespace Inspinia_MVC5.ViewModels
{
    public class EventChangerWithDetails
    {
        public string ChangerName { get; set; }
        public System.DateTime DTime { get; set; }
        public string KindEvent { get; set; }
        public List<GetDataEventsByChanger_Result> Details { get; set; }

        public EventChangerWithDetails(string changerName, DateTime dTime, string kindEvent, List<GetDataEventsByChanger_Result> details)
        {
            ChangerName = changerName;
            DTime = dTime;
            KindEvent = kindEvent;
            Details = details;
        }
    }
}