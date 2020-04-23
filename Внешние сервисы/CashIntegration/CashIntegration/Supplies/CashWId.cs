using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashIntegration.Supplies
{
    class CashWId
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public DateTime Date { get; set; }
        public List<CashIncome> CashIncome { get; set; }

        public CashWId()
        {
            CashIncome = new List<CashIncome>();
        }
    }
}
