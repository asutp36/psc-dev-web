﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashDataService.Models
{
    public class PayoutCashInsertionModel
    {
        public string TerminalCode { get; set; }
        public int M10 { get; set; }
        public int B50 { get; set; }
        public int B100 { get; set; }
        public int UserID { get; set; }
    }
}
