using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LeikaIntegration.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LeikaIntegration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
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



                return Ok();
            }
            catch(Exception e)
            {
                return StatusCode(500);
            }
        }

        [HttpPost]
        public IActionResult Start()
        {
            return Ok();
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