﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers.Supplies.ViewModels
{
    public class WashHappyHourViewModel
    {
        public string wash { get; set; }
        public List<PostHappyHourViewModel> posts { get; set; }
    }
}