using Inspinia_MVC5.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inspinia_MVC5.Helpers
{
    public class RatesWash
    {
        public string Wash { get; set; }
        public List<Rate> Rates { get; set; }        
    }

    public class Rate
    {
        public string Post { get; set; }
        public List<Price> Prices { get; set; }
    }
}