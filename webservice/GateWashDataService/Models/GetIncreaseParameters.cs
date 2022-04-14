using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashDataService.Models
{
    public class GetIncreaseParameters
    {
        public PagingParameter Paging { get; set; } = new PagingParameter();
        public DateTime StartDate { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
        public DateTime EndDate { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
        public string Terminal { get; set; } = null;
        public string Program { get; set; } = null;
        public bool OnlyBank { get; set; } = false;
        public bool OnlyCash { get; set; } = false;
        public bool OnlyCheque { get; set; } = false;
        public bool OnlyNotes { get; set; } = false;
    }
}
