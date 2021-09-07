using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers.Supplies.ViewModels
{
    public class SummaryManyWashes
    {
        public List<SummaryWash> washesSummaries { get; set; }
        public int sumIncreaseAllTime
        {
            get
            {
                int result = 0;
                foreach (SummaryWash s in washesSummaries)
                    result += s.increaseAllTime;
                return result;
            }
        }

        public int sumIncreaseThisMonth
        {
            get
            {
                int result = 0;
                foreach (SummaryWash s in washesSummaries)
                    result += s.increaseThisMonth;
                return result;
            }
        }

        public int sumIncreaseYesterday
        {
            get
            {
                int result = 0;
                foreach (SummaryWash s in washesSummaries)
                    result += s.increaseYesterday;
                return result;
            }
        }

        public int sumCollectLastMonth
        {
            get
            {
                int result = 0;
                foreach (SummaryWash s in washesSummaries)
                    result += s.collectLastMonth;
                return result;
            }
        }

        public int sumIncreaseAfterCollect
        {
            get
            {
                int result = 0;
                foreach (SummaryWash s in washesSummaries)
                    result += s.increaseAfterCollect;
                return result;
            }
        }

        public int sumIncreaseToday
        {
            get
            {
                int result = 0;
                foreach (SummaryWash s in washesSummaries)
                    result += s.increaseToday;
                return result;
            }
        }
    }
}
