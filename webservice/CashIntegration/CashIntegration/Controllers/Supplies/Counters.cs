using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CashIntegration.Controllers.Supplies
{
    public class Counters
    {
        public string NominalCode { get; set; }
        public int Val { get; set; }

        public Counters(string code, int val)
        {
            this.NominalCode = code;
            this.Val = val;
        }
    }
}