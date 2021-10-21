using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers.Supplies.ViewModels
{
    public class RatesModel : IEquatable<RatesModel>
    {
        public List<RateViewModel> rates { get; set; }

        public override bool Equals(object obj)
        {
            return obj is RatesModel && Equals(obj as RatesModel);
        }

        public bool Equals(RatesModel other)
        {
            if (other == null || other.rates == null)
                return false;

            foreach (RateViewModel r in rates)
                if (other.rates.Find(x => x.Equals(r)) == null)
                    return false;

            return true;
        }
    }
}
