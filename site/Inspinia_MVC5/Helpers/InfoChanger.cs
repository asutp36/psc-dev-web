using Inspinia_MVC5.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Inspinia_MVC5.Helpers;

namespace Inspinia_MVC5.Helpers
{
    public class InfoChanger
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
        public Sensor bill { get; set; } //купюрник
        public Sensor coiner { get; set; } //монетник
        public Sensor bank { get; set; }
        public Sensor oddMoney { get; set; } //сдача
        public Sensor hopper { get; set; } //воронка для монет
        public Sensor cards { get; set; }
        public Sensor issueCards { get; set; } //выпуск карт
        public Sensor fr { get; set; } //ФР
        public Sensor printCheck { get; set; } //печать чека
        public Device changer { get; set; }
    }
}