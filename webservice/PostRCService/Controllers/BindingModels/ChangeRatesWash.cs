﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostRCService.Controllers.BindingModels
{
    public class ChangeRatesWash
    {
        public string[] washes { get; set; }
        public FunctionRate[] rates { get; set; }
    }
}
