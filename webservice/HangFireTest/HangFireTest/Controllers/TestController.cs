using Hangfire;
using Hangfire.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HangFireTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private IBackgroundJobClient _backgroundJobs;

        public TestController(IBackgroundJobClient backgroundJobs)
        {
            _backgroundJobs = backgroundJobs;
        }

        [HttpGet("jobs/recurring")]
        public IActionResult GetRecurringJob()
        {
            List<RecurringJobDto> recurringJobs = JobStorage.Current.GetConnection().GetRecurringJobs().ToList();

            return Ok(recurringJobs);
        }

        [HttpPost("reccuring")]
        public IActionResult ReccuringJob()
        {
            RecurringJob.AddOrUpdate($"", () => Console.WriteLine(""), "0 0 0 * * *");
            return StatusCode(201);
        }
    }
}
