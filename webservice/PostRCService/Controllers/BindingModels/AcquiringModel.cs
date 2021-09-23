using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostRCService.Controllers.BindingModels
{
    public class AcquiringModel
    {
        public int BankAmountMin { get; set; }
        public int BankAmountMax { get; set; }
        public int BankAmountStep { get; set; }
    }
}
