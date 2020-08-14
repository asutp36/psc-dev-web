using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PostControllingService.Controllers.Supplies
{
    public class ChangerCounters
    {
        public int m10 { get; set; }
        public int b50 { get; set; }
        public int b100 { get; set; }
        public int b200 { get; set; }
        public int b500 { get; set; }
        public int b1000 { get; set; }
        public int b2000 { get; set; }
        public int box1_50 { get; set; }
        public int box2_100 { get; set; }
        public int box3_50 { get; set; }
        public int box4_100 { get; set; }
        public int badCards { get; set; }
        public int availableCards { get; set; }
        public string bill { get; set; }
        public string coiner { get; set; }
        public string bank { get; set; }
        public string oddMoney { get; set; }
        public string hopper { get; set; }
        public string cards { get; set; }
        public string issueCards { get; set; }
        public string fr { get; set; }
        public string printCheck { get; set; }
    }
}