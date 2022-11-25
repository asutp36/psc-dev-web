using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashDataService.Models
{
    public class GetClientActionsParameters
    {
        public SortingParameter Sorting { get; set; }
        public PagingParameter Paging { get; set; } = new PagingParameter();
        public DateTime StartDate { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
        public DateTime EndDate { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
        public string Terminal { get; set; } = null;
        public string Program { get; set; } = null;
    }
}
