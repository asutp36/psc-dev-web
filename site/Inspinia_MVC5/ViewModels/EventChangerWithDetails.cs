using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Inspinia_MVC5.Models;

namespace Inspinia_MVC5.ViewModels
{
    public class EventChangerWithDetails
    {
        public string ChangerCode { get; set; }
        public System.DateTime DTime { get; set; }
        public string KindEventCode { get; set; }
        public string KindEvent { get; set; }
        public List<GetDataEventsByChanger_Result> Details { get; set; }

        public EventChangerWithDetails(string changerCode, DateTime dTime, string kindEventCode, string kindEvent, List<GetDataEventsByChanger_Result> details)
        {
            ChangerCode = changerCode;
            DTime = dTime;
            KindEventCode = kindEventCode;
            KindEvent = kindEvent;
            Details = details;
        }
    }
}