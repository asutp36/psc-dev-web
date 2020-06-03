using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MobileIntegration.Controllers.Supplies
{
    public class StartPostBindingModel
    {
        public DateTime time_send { get; set; }
        public string hash { get; set; }
        public string card { get; set; }
        public string post { get; set; }
        public int balance { get; set; }
    }
}