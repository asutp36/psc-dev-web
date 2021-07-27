using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HangFireTest.Controllers.Models
{
    public class WhattsAppReportScheduleJobModel : WhattsAppReportJobModel
    {
        [Required]
        public string cronString { get; set; }
    }
}
