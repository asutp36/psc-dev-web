using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers.Supplies.ViewModels
{
    public class AcquiringModel : IEquatable<AcquiringModel>
    {
        public int BankAmountMin { get; set; }
        public int BankAmountMax { get; set; }
        public int BankAmountStep { get; set; }

        public override bool Equals(object obj)
        {
            return obj is AcquiringModel && Equals(obj as AcquiringModel);
        }

        public bool Equals(AcquiringModel other)
        {
            return this.BankAmountMin == other.BankAmountMin && this.BankAmountMax == other.BankAmountMax && this.BankAmountStep == other.BankAmountStep;
        }
    }
}
