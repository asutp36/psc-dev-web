﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostRCService.Controllers.BindingModels
{
    public class WashRates
    {
        public string washCode { get; set; }
        public List<PostRates> posts { get; set; }
    }
}
