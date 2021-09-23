using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers.Supplies.ViewModels
{
    public class PostAcquiringViewModel
    {
        public string Post { get; set; }
        public int BankAmountMin { get; set; }
        public int BankAmountMax { get; set; }
        public int BankAmountStep { get; set; }
    }
}
