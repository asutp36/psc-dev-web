using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers.Supplies.ViewModels
{
    public class SetRateResult
    {
        public string washCode { get; set; }
        public List<SetRateResultPost> posts { get; set; }
    }
}
