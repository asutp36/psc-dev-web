using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashIntegration.Supplies
{
    class Cash
    {
        public string Code { get; set; }
        public DateTime Date { get; set; }
        public List<CashIncome> CashIncome { get; set; }

        public Cash()
        {
            CashIncome = new List<CashIncome>();
        }
    }
}
