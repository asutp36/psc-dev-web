using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LeikaIntegration.Controllers.Supplies;
using LeikaIntegration.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LeikaIntegration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly ModelDbContext _model = new ModelDbContext();

        [HttpGet]
        public IActionResult State(string code)
        {
            try 
            {
                string postIp = GetPosIp(code);
                if (postIp.Equals(""))
                {
                    return NotFound();
                }

                Supplies.HttpResponse response = Supplies.HttpSender.SendGet("http://" + postIp + "");
                switch (response.StatusCode)
                {
                    case 0:
                        return StatusCode(424);

                    case (HttpStatusCode)423:
                        return StatusCode(423);

                    case HttpStatusCode.OK:
                        return Ok();

                    default:
                        return StatusCode(500);
                }
            }
            catch(Exception e)
            {
                return StatusCode(500);
            }
        }

        [HttpPost]
        public IActionResult Start()
        {
            try
            {
                return Ok();
            }
            catch(Exception e)
            {
                return StatusCode(500);
            }
        }

        private string GetPosIp(string code)
        {
            try
            {
                return _model.Device.Find(_model.Posts.Where(p => p.Qrcode.Equals(code)).FirstOrDefault().Iddevice).IpAddress;
            }
            catch (NullReferenceException)
            {
                return "";
            }
        }
    }
}