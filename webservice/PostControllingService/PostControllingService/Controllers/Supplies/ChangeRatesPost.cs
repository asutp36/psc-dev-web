using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PostControllingService.Controllers.Supplies
{
    public class ChangeRatesPost
    {
        public string PostCode { get; set; }
        public Price[] Rates { get; set; }
    }
}