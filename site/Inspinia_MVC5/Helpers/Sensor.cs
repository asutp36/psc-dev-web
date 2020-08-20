using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inspinia_MVC5.Helpers
{
    public class Sensor
    {
        public string devicecode { get; set; }
        public string devicename { get; set; }
        public string errlevel { get; set; }
        public List<string> errors { get; set; }
    }
}