using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashDataService.Models
{
    public class CardsRefill : CardIssuance
    {
        public int Amount { get; set; }
        public string Login { get; set; }
    }
}
