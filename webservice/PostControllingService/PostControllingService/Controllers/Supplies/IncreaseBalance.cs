using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PostControllingService.Controllers.Supplies
{
    public class IncreaseBalance
    {
        public string Post { get; set; }
        public DateTime DTime { get; set; }
        public int Amount { get; set; }
        public string Login { get; set; }
    }
}