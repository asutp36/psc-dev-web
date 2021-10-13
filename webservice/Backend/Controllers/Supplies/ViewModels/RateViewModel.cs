using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers.Supplies.ViewModels
{
    public class RateViewModel
    {
        public string Func { get; set; }
        public string Code { get; set; }
        public int Rate { get; set; }

        public bool Equals(RateViewModel r)
        {
            return this.Func == r.Func && this.Code == r.Code && this.Rate == r.Rate;
        }
    }
}
