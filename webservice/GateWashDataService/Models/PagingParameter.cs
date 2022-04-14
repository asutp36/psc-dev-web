﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashDataService.Models
{
    public class PagingParameter
    {
        public int PageSize { get; set; } = 50;
        public int PageNumber { get; set; } = 1;

    }
}
