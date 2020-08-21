using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PostControllingService.Controllers.Supplies
{
    public class ChangerState
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
        public int box5_10 { get; set; }
        public int badCards { get; set; }
        public int availableCards { get; set; }
        public ChangerDevice bill { get; set; }
        public ChangerDevice coiner { get; set; }
        public ChangerDevice bank { get; set; }
        public ChangerDevice oddMoney { get; set; }
        public ChangerDevice hopper { get; set; }
        public ChangerDevice cards { get; set; }
        public ChangerDevice issueCards { get; set; }
        public ChangerDevice fr { get; set; }
        public ChangerDevice printCheck { get; set; }
    }
}