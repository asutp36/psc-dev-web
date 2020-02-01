using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace PostControllingService.Controllers.Supplies
{
    public class IncreaseBalance
    {
        public string Post { get; set; }
        public string DTime { get; set; }
        public int Amount { get; set; }
        public string Login { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}