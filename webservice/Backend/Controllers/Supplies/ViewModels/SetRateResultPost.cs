using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers.Supplies.ViewModels
{
    public class SetRateResultPost
    {
        public string postCode { get; set; }
        public HttpResponse result { get; set; }
    }
}
