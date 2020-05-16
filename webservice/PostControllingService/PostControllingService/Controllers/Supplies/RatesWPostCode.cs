using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PostControllingService.Controllers.Supplies
{
    public class RatesWPostCode
    {
        public string Post { get; set; }
        public List<FunctionRate> Rates { get; set; }
    } 
}