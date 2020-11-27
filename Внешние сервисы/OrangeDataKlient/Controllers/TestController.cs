using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OrangeDataKlient.Controllers.Supplies;

namespace OrangeDataKlient.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        PostDocuments example = new PostDocuments { id = "2_loc_z5bYWHvD", group = "Main", inn = "1234567890", key = "1234567890" };
        [HttpGet]
        public IActionResult Get()
        {
            string data = JsonConvert.SerializeObject(example);

            Supplies.HttpResponse response = HttpSender.SendPost("https://apip.orangedata.ru:2443/api/v2/documents", data, Signature.ComputeSignature(data));
            return Ok(response);
        }
    }
}
