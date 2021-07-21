﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HangFireTest.Controllers.Models;
using Hangfire;
using Hangfire.SqlServer;
using HangFireTest.JobHelpers.WhattAppReportSender;

namespace HangFireTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        private IBackgroundJobClient _backgroundJobs;

        public JobsController(IBackgroundJobClient backgroundJobs)
        {
            this._backgroundJobs = backgroundJobs;
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Создать немедленную задачу")]
        [SwaggerResponse(200, Description = "Задача создана")]
        #endregion
        [HttpPost("imidiate")]
        public async Task<IActionResult> CreateImidiateJobAsync(WhattsAppReportImidateJobModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            return Ok();
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Запустить немедленную задачу")]
        [SwaggerResponse(200, Description = "Задача запущена")]
        #endregion
        [HttpGet("imidiate/{recipient}")]
        public async Task<IActionResult> RunImidiateJobAsync(int recipient)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            _backgroundJobs.Enqueue(() => WhattsAppReportSender.CreateReportJob(recipient));

            return Ok();
        }
    }
}