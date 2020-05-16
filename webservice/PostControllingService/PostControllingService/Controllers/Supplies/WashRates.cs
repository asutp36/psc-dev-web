using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PostControllingService.Controllers.Supplies
{
    public class WashRates
    {
        public string Wash { get; set; }
        public List<RatesWPostCode> Rates { get; set; }
    }
}