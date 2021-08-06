using HangFireTest.Controllers.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HangFireTest.Controllers.Helpers;
using Newtonsoft.Json;

namespace HangFireTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatsController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            Helpers.HttpResponse response = HttpSender.SendGet("https://api.chat-api.com/instance27633/dialogs?token=0qgid5wjmhb8vw7d");

            if(response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return StatusCode(424, response.ResultMessage);
            }

            WhattsAppChatList list = JsonConvert.DeserializeObject<WhattsAppChatList>(response.ResultMessage);

            return Ok(list.dialogs);
        }
    }
}
