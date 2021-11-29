using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers.Supplies.ViewModels
{
    public class FiscalPercentModel : IEquatable<FiscalPercentModel>
    {
        public int FiscalizationPercentage { get; set; }

        public override bool Equals(object obj)
        {
            return obj is FiscalPercentModel && Equals(obj as FiscalPercentModel);
        }

        public bool Equals(FiscalPercentModel other)
        {
            if (other == null)
                return false;

            return this.FiscalizationPercentage == other.FiscalizationPercentage;
        }
    }
}
