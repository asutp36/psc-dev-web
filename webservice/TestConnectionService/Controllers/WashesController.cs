using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestConnectionService.Services;
using TestConnectionService.Models;

namespace TestConnectionService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WashesController : ControllerBase
    {
        private CacheService _cache;

        public WashesController(CacheService cache)
        {
            _cache = cache;
        }

        [HttpGet]
        public IActionResult GetConnection()
        {
            try
            {
                DateTime now = DateTime.Now;
                List<string> result = new List<string>();

                string res = _cache.GetLastConnection("13-1");

                return Ok(res);
            }
            catch(Exception e)
            {
                return StatusCode(500, e);
            }
        }

        private List<string> GetIPs(string washCode)
        {
            ModelDbContext model = new ModelDbContext();
            List<string> result = new List<string>();

            List<int?> devices = model.Posts.Where(p => p.Idwash == model.Washes.Where(w => w.Code == washCode).FirstOrDefault().Idwash).Select(d => d.Iddevice).ToList();
            foreach (int? d in devices)
            {
                if(d != null)
                {
                    result.Add(model.Devices.Find(d).IpAddress);
                }
            }
            return result;
        }
    }
}
