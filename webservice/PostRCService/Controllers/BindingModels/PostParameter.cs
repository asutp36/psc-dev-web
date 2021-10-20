﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostRCService.Controllers.BindingModels
{
    public class PostParameter<T>
    {
        public string postCode { get; set; }
        public T value { get; set; }
    }
}