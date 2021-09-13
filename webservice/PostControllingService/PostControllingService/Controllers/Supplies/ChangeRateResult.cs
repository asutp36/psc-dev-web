using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PostControllingService.Controllers.Supplies
{
    public class ChangeRateResult
    {
        public string washCode { get; set; }
        public List<ChangeRateResultPost> posts { get; set; }
    }
}