﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashDataService.Models
{
    public class Error
    {
        public string errorMessage { get; set; }
        public string errorType { get; set; }
        public string alert { get; set; }
        public string errorCode { get; set; }
    }
}