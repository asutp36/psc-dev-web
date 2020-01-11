using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inspinia_MVC5.Helpers
{
    public class ChangePricesData
    {
        public List<int> posts { get; set; }

        public List<Price> prices { get; set; }

        public ChangePricesData(List<int> posts, List<List<string>> prices)
        {
            this.posts = posts;

            this.prices = new List<Price>();

            foreach (List<string> i in prices)
            {
                this.prices.Add(new Price(i[0], int.Parse(i[1])));                
            }
        }
    }

    public class Price
    {
        public string function { get; set; }

        public int price { get; set; }

        public Price(string f, int price)
        {
            this.function = f;
            this.price = price;
        }
    }
}