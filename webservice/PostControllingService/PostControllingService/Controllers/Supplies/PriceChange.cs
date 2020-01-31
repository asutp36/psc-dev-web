using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PostControllingService.Controllers.Supplies
{
    public class PricesChange
    {
        public int[] posts { get; set; }
        public int[] prices { get; set; }

        public PricesChange(int[] w, int[] p)
        {
            this.posts = w;
            this.prices = p;
        }
    }
}