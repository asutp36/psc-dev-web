using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MobileIntegration.Controllers.Supplies
{
    public class StopPostBindingModel
    {
        public DateTime time_send { get; set; }
        public string card { get; set; }
        public string post { get; set; }
        public double balance { get; set; }
    }
}