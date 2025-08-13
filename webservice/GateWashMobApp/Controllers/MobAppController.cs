using GateWashMobApp.Infrastructure;
using GateWashMobApp.Models.GateWashDb;
using GateWashMobApp.Models.MobAppModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GateWashMobApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MobAppController : ControllerBase
    {
        private readonly GateWashDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<MobAppController> _logger;

        public MobAppController(GateWashDbContext context, ILogger<MobAppController> logger, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        [SwaggerOperation(Summary = "Запрос суммы к оплате на терминале")]
        [SwaggerResponse(200, type: typeof(decimal))]
        [SwaggerResponse(400, "Некорректные входные параметры")]
        [SwaggerResponse(401, "Хэш не прошёл проверку")]
        [SwaggerResponse(404, "Пост не найден")]
        [SwaggerResponse(423, "Оплата невозможна, сумма оплаты 0, терминал не ждёт оплаты")]
        [SwaggerResponse(424, "Нет связи с постом")]
        [SwaggerResponse(500, "Внутренняя ошибка")]
        [HttpPost("get_teminal_amount")]
        public async Task<IActionResult> GetTerminalAmount(GetTerminalAmountRequest request)
        {
            _logger.LogDebug("pay: Запуск с параметрами {@Parameters}", request);

            if (!BCryptHasher.CheckHashCode(request.hash, request.time_send))
            {
                return Unauthorized();
            }

            if (string.IsNullOrEmpty(request.post))
            {
                return BadRequest("Некорректное значение поста");
            }

            if (!PhoneNumberIsValid(request.phone))
            {
                return BadRequest("Некорректное значение телефона");
            }

            var device = await GetDeviceByCodeAsync(request.post);
            if (device is null) 
            {
                _logger.LogWarning("get_teminal_amount: Не нашёлся пост по qr коду {QrCode}", request.post);
                return NotFound("Пост не найден");
            }

            if (string.IsNullOrEmpty(device.IpAddress))
            {
                _logger.LogWarning("get_teminal_amount: У поста {DeviceCode} пустой ip адрес", device.Code);
                return StatusCode(424);
            }

            var deviceQuery = new GetTerminalAmountQuery()
            {
                ClientPhone = request.phone
            };

            var deviceResponse = await SendGetAmountToDeviceAsync(device.IpAddress, deviceQuery);

            if(deviceResponse is null)
            {
                _logger.LogWarning("get_teminal_amount: Нет связи с постом {DeviceCode}", device.Code);
                return StatusCode(424);
            }

            if (deviceResponse.IsSuccessStatusCode)
            {
                decimal? amount = await GetAmountFromResponse(deviceResponse);

                if (amount is null)
                {
                    // no amount in response
                    _logger.LogWarning("get_teminal_amount: В ответе поста {DeviceCode} нет суммы", device.Code);
                }

                return Ok(amount.Value);
            }
            else
            {
                return deviceResponse.StatusCode switch
                {
                    (HttpStatusCode)406 => StatusCode(423),
                    _ => StatusCode(424),
                };
            }
        }

        [SwaggerOperation(Summary = "Оплата через мобильное приложение")]
        [SwaggerResponse(200, Description = "В теле сумма", Type = typeof(decimal))]
        [SwaggerResponse(400, Description = "Некорректные входные параметры")]
        [SwaggerResponse(401, Description = "Хэш не прошёл проверку")]
        [SwaggerResponse(404, Description = "Пост не найден")]
        [SwaggerResponse(423, Description = "Оплата невозможна, терминал не ждёт оплаты, телефон или сумма не совпадают с указанными")]
        [SwaggerResponse(424, Description = "Нет связи с постом")]
        [SwaggerResponse(500, Description = "Внутренняя ошибка")]
        [HttpPost("pay")]
        public async Task<IActionResult> PayToTerminal(PayRequest request)
        {
            _logger.LogDebug("pay: Запуск с параметрами {@Parameters}", request);

            if (!BCryptHasher.CheckHashCode(request.hash, request.time_send))
            {
                return Unauthorized();
            }

            if (string.IsNullOrEmpty(request.post))
            {
                return BadRequest("Некорректное значение поста");
            }

            if (!PhoneNumberIsValid(request.phone))
            {
                return BadRequest("Некорректное значение телефона");
            }

            if (request.amount <= 0)
            {
                return BadRequest("Некорректное значение суммы");
            }

            var device = await GetDeviceByCodeAsync(request.post);
            if (device is null)
            {
                _logger.LogWarning("pay: Не нашёлся пост по qr коду {QrCode}", request.post);
                return NotFound("Пост не найден");
            }

            if (string.IsNullOrEmpty(device.IpAddress))
            {
                _logger.LogWarning("pay: У поста {DeviceCode} пустой ip адрес", device.Code);
                return StatusCode(424);
            }

            var deviceRequest = new PayDeviceRequest()
            {
                ClientPhone = request.phone,
                Amount = request.amount,
            };

            var deviceResponse = await SendPayToDevice(device.IpAddress, deviceRequest);

            if (deviceResponse is null)
            {
                _logger.LogWarning("pay: Нет связи с постом {DeviceCode}", device.Code);
                return StatusCode(424);
            }

            if (deviceResponse.IsSuccessStatusCode)
            {
                decimal? amount = await GetAmountFromResponse(deviceResponse);

                if (amount is null)
                {
                    // no amount in response
                    _logger.LogWarning("pay: В ответе поста {DeviceCode} нет суммы", device.Code);
                }

                return Ok(amount.Value);
            }
            else
            {
                return deviceResponse.StatusCode switch
                {
                    (HttpStatusCode)406 => StatusCode(423),
                    _ => StatusCode(424),
                };
            }
        }

        private Task<Device> GetDeviceByCodeAsync(string code)
        {
            List<string> deviceTypeCodes = new List<string>() { "pay", "typedExit" };
            return _context.Device.Where(d => deviceTypeCodes.Contains(d.IddeviceTypeNavigation.Code) && d.Code.Equals(code)).FirstOrDefaultAsync();
        }

        private bool PhoneNumberIsValid(string phone)
        {
            return !string.IsNullOrEmpty(phone) &&
                   phone.StartsWith("7") &&
                   phone.Length == 11;

        }

        private async Task<HttpResponseMessage?> SendGetAmountToDeviceAsync(string ip, GetTerminalAmountQuery query)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                return await client.GetAsync($"http://{ip}/api/gate/pay/amount/get?ClientPhone={query.ClientPhone}");
            }
            catch(HttpRequestException ex)
            {
                return null;
            }
        }

        private async Task<HttpResponseMessage?> SendPayToDevice(string ip, PayDeviceRequest request)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var jsonContent = JsonConvert.SerializeObject(request);

                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                return await client.PostAsync($"http://{ip}/api/gate/pay/amount/increase/mobapp", content);
            }
            catch (HttpRequestException ex)
            {
                return null;
            }
        }

        private async Task<decimal?> GetAmountFromResponse(HttpResponseMessage response)
        {
            var stringContent = await response.Content.ReadAsStringAsync();
            if(decimal.TryParse(stringContent, out decimal amount))
            {
                return amount;
            }

            return null;
        }
    }
}
