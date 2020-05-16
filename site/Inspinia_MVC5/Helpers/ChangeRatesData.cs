using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inspinia_MVC5.Helpers
{
    public class ChangeRatesData
    {
        public List<string> Washes { get; set; }
        public List<Price> Rates { get; set; }

        public ChangeRatesData(List<string> Washes, List<List<string>> Rates)
        {
            this.Washes = Washes;
            this.Rates = new List<Price>();

            foreach (List<string> i in Rates)
            {
                this.Rates.Add(new Price() { 
                    Code = i[0],
                    Rate = int.Parse(i[1])
                });                
            }
        }
    }

    public class Price
    {
        public string Func { get; set; }
        public string Code { get; set; }
        public int Rate { get; set; }
    }
}