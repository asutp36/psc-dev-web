﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashDataService.Models
{
    public class IncreaseModel
    {
        public DateTime DTime { get; set; }
        public string Terminal { get; set; }
        public string Program { get; set; }
        public int Bank { get; set; }
        public int Cash { get; set; }
        public int Amount { get { return this.Bank + this.Cash - this.Payout; } set { } }
        public int Payout { get; set; }
        public bool Cheque { get; set; }
        public string Note { get; set; }
    }
}