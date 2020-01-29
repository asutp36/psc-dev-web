using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CashIntegration.Controllers.Supplies
{
    public class Cash
    {
            public int TerminalId { get; set; }
            public string Date { get; set; }
            public Income[] CashIncome { get; set; }
            public Counters[] TotalCounters { get; set; }
    }
}