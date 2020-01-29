using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CashIntegration.Controllers.Supplies
{
    public class Income
    {
        public string NominalCode { get; set; }
        public int Val { get; set; }

        public Income(string code, int val)
        {
            this.NominalCode = code;
            this.Val = val;
        }
    }
}