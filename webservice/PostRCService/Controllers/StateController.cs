using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PostRCService.Controllers.BindingModels;
using PostRCService.Models.GateWashContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PostRCService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StateController : ControllerBase
    {
        private readonly ILogger<StateController> _logger;
        private readonly GateWashDbContext _context;

        public StateController(ILogger<StateController> logger, GateWashDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpPost("start")]
        public async Task<IActionResult> Start([FromBody] PostStartDicountModel model)
        {
            if (!IsDeviceExists(model.DeviceCode))
            {
                _logger.LogError($"Не найден девайс {model.DeviceCode}" + Environment.NewLine);
                return NotFound($"Не найден девайс {model.DeviceCode}");
            }

            string deviceIP = GetDeviceIP(model.DeviceCode);
            if (string.IsNullOrEmpty(deviceIP))
            {
                _logger.LogError($"Не найден IP поста {model.DeviceCode}");
                return StatusCode(503, $"Не найден IP поста {model.DeviceCode}");
            }

            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri($"http://{deviceIP}/api/post/discount");

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "");
            string json = JsonConvert.SerializeObject(new PostStartingDiscountParameters { Discount = model.Discount, ClientPhone = model.ClientPhone });

            if (!string.IsNullOrEmpty(json))
            {
                request.Content = new StringContent(json, Encoding.UTF8);
            }

            try 
            {
                HttpResponseMessage response = await httpClient.SendAsync(request);

                string content = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    return Ok();
                }
                else
                {
                    _logger.LogError($"Ответ от поста {model.DeviceCode} не ок. StatusCode: {response.StatusCode}, Message: {content}");
                    return StatusCode(424, "Ответ не 200");
                }

            }
            catch(HttpRequestException e) 
            {
                _logger.LogError($"Не удалось запустить пост. HttpRequestException: {e.Message}");
                return StatusCode(424, "Нет связи с постом");
            }
        }

        private bool IsDeviceExists(string deviceCode)
        {
            return _context.Devices.Where(d => d.Code == deviceCode).FirstOrDefault() != null;
        }

        private string GetDeviceIP(string deviceCode)
        {
            return _context.Devices.Where(d => d.Code == deviceCode).Select(d => d.IpAddress).FirstOrDefault();
        }
    }
}
