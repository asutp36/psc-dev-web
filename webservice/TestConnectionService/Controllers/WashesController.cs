using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestConnectionService.Controllers.Supplies;
using TestConnectionService.Models;
using HttpResponse = TestConnectionService.Controllers.Supplies.HttpResponse;

namespace TestConnectionService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WashesController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetConnection()
        {
            try
            {
                DateTime now = DateTime.Now;
                List<string> result = new List<string>();
                for (int i = 0; i < 100; i++)
                {
                    List<string> ips = GetIPs("М13");
                    foreach (string ip in ips)
                    {
                        if (ip == null)
                            continue;
                        HttpResponse response = HttpSender.SendGet("http://" + ip + "/api/post/heartbeat");
                        result.Add(response.StatusCode.ToString());
                    }
                }

                return Ok((DateTime.Now - now).ToString());
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
