using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HangFireTest.Controllers.Models
{
    public class WhattsAppReportJobModel
    {
        [Required]
        public int recipient { get; set; }
        [Required]
        public string chatId { get; set; }
        [Required]
        public string washCode { get; set; }
    }
}
