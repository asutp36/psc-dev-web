﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashDataService.Models
{
    public class PayoutEventKind : PayoutInsertion
    {
        public string EventKind { get; set; }
    }
}