using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PostRCService.Controllers.Helpers;
using PostRCService.Models;
using PostRCService.Services;
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
    public class RefillGateWashController : ControllerBase
    {
        private readonly ILogger<RefillGateWashController> _logger;
        private readonly GateWashService _gateWashService;

        public RefillGateWashController(ILogger<RefillGateWashController> logger, GateWashService gateWashService)
        {
            _logger = logger;
            _gateWashService = gateWashService;
        }

        [HttpPost("cards")]
        public async Task<IActionResult> RefillCards(TerminalCardsInsertionModel model)
        {
            string ip = await _gateWashService.GetDeviceIp(model.TerminalCode);
            if (ip == null)
            {
                _logger.LogError($"Не найден ip девайса {model.TerminalCode}");
                throw new CustomStatusCodeException(System.Net.HttpStatusCode.NotFound, $"Не найден ip девайса {model.TerminalCode}", "");
            }

            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri($"http://{ip}/");

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "api/gate/cards/increase");
            string json = JsonConvert.SerializeObject(model);

            if (!string.IsNullOrEmpty(json))
            {
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            _logger.LogInformation("Headers:");
            foreach (var h in request.Headers)
            {
                _logger.LogInformation($"{h.Key}: {h.Value}");
            }

            try
            {
                _logger.LogInformation($"HTTP url: {request.Method} {httpClient.BaseAddress}{request.RequestUri}");
                _logger.LogInformation($"Body: {json}");
                HttpResponseMessage response = await httpClient.SendAsync(request);

                string content = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    return Ok();
                }
                else
                {
                    _logger.LogError($"Ответ от поста {model.TerminalCode} не ок. StatusCode: {response.StatusCode}, Message: {content}");
                    throw new CustomStatusCodeException((System.Net.HttpStatusCode)424, "Ответ не 200", content);
                }

            }
            catch (HttpRequestException e)
            {
                _logger.LogError($"Не удалось запустить пост. HttpRequestException: {e.Message}");
                throw new CustomStatusCodeException((System.Net.HttpStatusCode)424, "Нет связи с постом", e.Message);
            }
        }

        [HttpPost("payout")]
        public async Task<IActionResult> RefillPayout(PayoutCashInsertionModel model)
        {
            string ip = await _gateWashService.GetDeviceIp(model.terminalCode);
            if (ip == null)
            {
                _logger.LogError($"Не найден ip девайса {model.terminalCode}");
                throw new CustomStatusCodeException(System.Net.HttpStatusCode.NotFound, $"Не найден ip девайса {model.terminalCode}", "");
            }

            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri($"http://{ip}/");

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "api/gate/payout/increase");
            string json = JsonConvert.SerializeObject(model);

            if (!string.IsNullOrEmpty(json))
            {
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            _logger.LogInformation("Headers:");
            foreach (var h in request.Headers)
            {
                _logger.LogInformation($"{h.Key}: {h.Value}");
            }

            try
            {
                _logger.LogInformation($"HTTP url: {request.Method} {httpClient.BaseAddress}{request.RequestUri}");
                _logger.LogInformation($"Body: {json}");
                HttpResponseMessage response = await httpClient.SendAsync(request);

                string content = await response.Content.ReadAsStringAsync();
                //if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                //{
                //    _logger.LogError($"KeyError на терминале");
                //    throw new CustomStatusCodeException();
                //}

                if (response.IsSuccessStatusCode)
                {
                    return Ok();
                }
                else
                {
                    _logger.LogError($"Ответ от поста {model.terminalCode} не ок. StatusCode: {response.StatusCode}, Message: {content}");
                    throw new CustomStatusCodeException((System.Net.HttpStatusCode)424, "Ответ не 200", content);
                }

            }
            catch (HttpRequestException e)
            {
                _logger.LogError($"Не удалось запустить пост. HttpRequestException: {e.Message}");
                throw new CustomStatusCodeException((System.Net.HttpStatusCode)424, "Нет связи с постом", e.Message);
            }
        }
    }
}
