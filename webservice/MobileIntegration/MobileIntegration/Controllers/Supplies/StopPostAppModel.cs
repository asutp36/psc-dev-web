using System;

namespace MobileIntegration.Controllers.Supplies
{
    public class StopPostAppModel
    {
        public DateTime time_send { get; set; }
        public string hash { get; set; }
        public string card { get; set; }
        public string post { get; set; }
    }
}