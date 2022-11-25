using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashDataService.Models
{
    public class PayoutInsertion : Payout
    {
        public string login { get; set; }
    }
}
