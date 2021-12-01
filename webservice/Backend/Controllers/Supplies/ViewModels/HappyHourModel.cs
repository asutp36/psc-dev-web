using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers.Supplies.ViewModels
{
    public class HappyHourModel : IEquatable<HappyHourModel>
    {
        public int HappyHourBeg { get; set; }
        public int HappyHourEnd { get; set; }
        public int HappyHourSale { get; set; }

        public override bool Equals(object obj)
        {
            return obj is HappyHourModel && Equals(obj as HappyHourModel);
        }

        public bool Equals(HappyHourModel other)
        {
            if (other == null)
                return false;

            return this.HappyHourBeg == other.HappyHourBeg && this.HappyHourEnd == other.HappyHourEnd && this.HappyHourSale == other.HappyHourSale;
        }
    }
}
