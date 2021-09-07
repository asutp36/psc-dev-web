using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers.Supplies
{
    public class Summary
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int increaseAllTime { get; set; }
        public int increaseThisMonth { get; set; }
        public int increaseYesterday { get; set; }
        public int collectLastMonth { get; set; }
        public int increaseAfterCollect { get; set; }
        public int increaseToday { get; set; }
    }
}
