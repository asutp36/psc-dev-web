using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashIntegration.Supplies
{
    class CashIncome
    {
        public string NominalCode { get; set; }
        public int Val { get; set; }

        public CashIncome(string nominal, int val)
        {
            this.NominalCode = nominal;
            this.Val = val;
        }
    }
}
