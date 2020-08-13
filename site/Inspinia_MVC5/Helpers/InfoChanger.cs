using Inspinia_MVC5.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
        public int badCards { get; set; }
        public int availableCards { get; set; }
        public string bill { get; set; } //купюрник
        public string coiner { get; set; } //монетник
        public string bank { get; set; }
        public string oddMoney { get; set; } //сдача
        public string hopper { get; set; } //воронка для монет
        public string cards { get; set; }
        public string issueCards { get; set; } //выпуск карт
        public string fr { get; set; } //ФР
        public string printCheck { get; set; } //печать чека
        public Device changer { get; set; }

        public InfoChanger(
            int m10, 
            int b50, 
            int b100, 
            int b200, 
            int b500,
            int b1000,
            int b2000,
            int box1_50,
            int box2_100,
            int box3_50,
            int box4_100,
            int badCards,
            int availableCards,
            string bill,
            string coiner,
            string bank,
            string oddMoney,
            string hopper,
            string cards,
            string issueCards,
            string fr,
            string printCheck,
            Device changer
            )
        {
            this.m10 = m10;
            this.b50 = b50;
            this.b100 = b100;
            this.b200 = b200;
            this.b500 = b500;
            this.b1000 = b1000;
            this.b2000 = b2000;
            this.box1_50 = box1_50;
            this.box2_100 = box2_100;
            this.box3_50 = box3_50;
            this.box4_100 = box4_100;
            this.badCards = badCards;
            this.availableCards = availableCards;
            this.bill = bill;
            this.coiner = coiner;
            this.bank = bank;
            this.oddMoney = oddMoney;
            this.hopper = hopper;
            this.cards = cards;
            this.issueCards = issueCards;
            this.fr = fr;
            this.printCheck = printCheck;
            this.changer = changer;
        }
    }
}