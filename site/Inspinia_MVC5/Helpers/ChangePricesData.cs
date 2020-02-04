using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inspinia_MVC5.Helpers
{
    public class ChangePricesData
    {
        public List<string> Posts { get; set; }

        public List<Price> Prices { get; set; }

        public ChangePricesData(List<string> Posts, List<List<string>> Prices)
        {
            this.Posts = Posts;

            this.Prices = new List<Price>();

            foreach (List<string> i in Prices)
            {
                this.Prices.Add(new Price(i[0], int.Parse(i[1])));                
            }
        }
    }

    public class Price
    {
        public string Function { get; set; }

        public int Rate { get; set; }

        public Price(string Function, int Rate)
        {
            this.Function = Function;
            this.Rate = Rate;
        }
    }
}