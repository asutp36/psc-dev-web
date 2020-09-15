using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inspinia_MVC5.Helpers
{
    public class ChangerSumData
    {
        public string changerCode { get; set; }
        public string changerName { get; set; }
        public string sumIncrease { get; set; }
        public string sumOut { get; set; }
        public string sumCards { get; set; }
        public string boxIncrease { get; set; }
        public string boxChange { get; set; }
        public string boxCards { get; set; }

        public ChangerSumData(
            string changerCode, 
            string changerName, 
            string sumIncrease, 
            string sumOut,
            string sumCards,
            string boxIncrease,
            string boxChange,
            string boxCards
            )
        {
            this.changerCode = changerCode;
            this.changerName = changerName;
            this.sumIncrease = sumIncrease;
            this.sumOut = sumOut;
            this.sumCards = sumCards;
            this.boxIncrease = boxIncrease;
            this.boxChange = boxChange;
            this.boxCards = boxCards;
        }
    }
}