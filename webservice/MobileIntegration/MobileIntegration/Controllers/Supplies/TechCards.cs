using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MobileIntegration.Controllers.Supplies
{
    public class TechCards
    {
        public List<string> cleanUp { get; set; }
        public List<string> collect { get; set; }
        public List<string> doors { get; set; }
        public List<string> service { get; set; }
    }
}