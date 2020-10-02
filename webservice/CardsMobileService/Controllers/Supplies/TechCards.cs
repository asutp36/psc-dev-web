using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardsMobileService.Controllers.Supplies
{
    public class TechCards
    {
        public List<string> cleanUp { get; set; }
        public List<string> collect { get; set; }
        public List<string> doors { get; set; }
        public List<string> service { get; set; }

        public TechCards()
        {
            cleanUp = new List<string>();
            collect = new List<string>();
            doors = new List<string>();
            service = new List<string>();
        }
    }
}

