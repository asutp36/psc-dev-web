using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PostControllingService.Controllers.Supplies
{
    public class ChangeRateResultPost
    {
        public string postCode { get; set; }
        public HttpSenderResponse result { get; set; }
    }
}