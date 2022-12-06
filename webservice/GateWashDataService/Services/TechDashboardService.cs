using GateWashDataService.Exceptions;
using GateWashDataService.Models;
using GateWashDataService.Models.GateWashContext;
using GateWashDataService.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GateWashDataService.Services
{
    public class TechDashboardService
    {
        private readonly GateWashDbContext _context;
        private readonly WashesRepository _washesRepository;
        private readonly ILogger<TechDashboardService> _logger;

        public TechDashboardService(GateWashDbContext context, WashesRepository washesRepository, ILogger<TechDashboardService> logger)
        {
            _context = context;
            _washesRepository = washesRepository;
            _logger = logger;
        }

        public async Task InsertPayoutCash()
        {

        }

        public async Task<List<WashWithTerminalsActions>> GetWashesWithTerminalsActions(IEnumerable<string> washCodes)
        {
            try
            {
                List<WashWithTerminalsActions> washes = await _context.Washes.Where(o => washCodes.Contains(o.Code))
                    .Include(o => o.Terminals).ThenInclude(o => o.IddeviceNavigation).ThenInclude(o => o.IddeviceTypeNavigation).ThenInclude(o => o.DeviceTypeAction)
                    .Select(o => new WashWithTerminalsActions
                    {
                        IdWash = o.Idwash,
                        Code = o.Code,
                        Name = o.Name,
                        Address = o.Address,
                        Terminals = o.Terminals.Select(e => new TerminalWithActions
                        {
                            IdTerminal = e.Idterminal,
                            Code = e.IddeviceNavigation.Code,
                            Name = e.IddeviceNavigation.Name,
                            Type = new TerminalType() { Code = e.IddeviceNavigation.IddeviceTypeNavigation.Code, Name = e.IddeviceNavigation.IddeviceTypeNavigation.Name },
                            InsertAction = e.IddeviceNavigation.IddeviceTypeNavigation.DeviceTypeAction.InsertPayoutCash ? "cash" : e.IddeviceNavigation.IddeviceTypeNavigation.DeviceTypeAction.InsertWashCards ? "cards" : null
                        })
                    }).ToListAsync();

                return washes;
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public async Task<List<RegionWithWashTerminalActions>> GetRegionsWithWashesTerminalAction(IEnumerable<string> washCodes)
        {
            var regionCodes = await _washesRepository.GetRegionCodessByWashCodes(washCodes);

            var result = await _context.Regions.Include(o => o.Washes.Where(e => washCodes.Contains(e.Code) && e.Terminals.Count() > 0))
                .ThenInclude(o => o.Terminals)
                .ThenInclude(o => o.IddeviceNavigation)
                .ThenInclude(o => o.IddeviceTypeNavigation)
                .ThenInclude(o => o.DeviceTypeAction)
                .Where(o => regionCodes.Contains(o.Code))
                .Select(o => new RegionWithWashTerminalActions
                {
                    IdRegion = o.Idregion,
                    Code = o.Code,
                    Name = o.Name,
                    Washes = o.Washes.Select(e => new WashWithTerminalsActions 
                    {
                        IdWash = e.Idwash,
                        Code = e.Code,
                        Name = e.Name,
                        Address = e.Address
                    })
                }).ToListAsync();
            return result;
        }

        public async Task<WashWithTerminalsActions> GetWashWithTerminalsActionsByCode(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                _logger.LogError($"Код мойки не задан");
                throw new CustomStatusCodeException(System.Net.HttpStatusCode.BadRequest, "Не задан код мойки", "Код мойки равен null или пустая строка");
            }

            if (!(await _context.Washes.AnyAsync(o => o.Code == code)))
            {
                _logger.LogError($"Не найдена мойка {code}");
                throw new CustomStatusCodeException(System.Net.HttpStatusCode.NotFound, "Нет такой мойки", $"Мойка {code} не найдена");
            }

            var wash = await _context.Washes.Where(o => o.Code == code)
                .Include(o => o.Terminals)
                .ThenInclude(o => o.IddeviceNavigation)
                .ThenInclude(o => o.IddeviceTypeNavigation)
                .ThenInclude(o => o.DeviceTypeAction)
                .Select(o => new WashWithTerminalsActions
                {
                    IdWash = o.Idwash,
                    Code = o.Code,
                    Name = o.Name,
                    Address = o.Address,
                    Terminals = o.Terminals.Where(t => t.IddeviceNavigation.IddeviceTypeNavigation.DeviceTypeAction.InsertPayoutCash || t.IddeviceNavigation.IddeviceTypeNavigation.DeviceTypeAction.InsertWashCards).Select(e => new TerminalWithActions
                    {
                        IdTerminal = e.Idterminal,
                        Code = e.IddeviceNavigation.Code,
                        Name = e.IddeviceNavigation.Name,
                        Type = new TerminalType() { Code = e.IddeviceNavigation.IddeviceTypeNavigation.Code, Name = e.IddeviceNavigation.IddeviceTypeNavigation.Name },
                        InsertAction = e.IddeviceNavigation.IddeviceTypeNavigation.DeviceTypeAction.InsertPayoutCash ? "cash" : e.IddeviceNavigation.IddeviceTypeNavigation.DeviceTypeAction.InsertWashCards ? "cards" : null
                    })
                }).FirstOrDefaultAsync();

            return wash;
        }

        public async Task<HttpResponseMessage> SendNotificationPayoutInsertion(PayoutCashInsertionModel cash)
        {
            var content = new { chatId = "-650370220", body = $"{cash.TerminalCode}: Добавлена сдача m10={cash.M10}, b50={cash.B50}, b100={cash.B100}" };

            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("http://cwmon.ru/notify/");

            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, "api/notify/message-group");
            requestMessage.Content = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");

            HttpResponseMessage result = await httpClient.SendAsync(requestMessage);

            return result;
        }

        public async Task<HttpResponseMessage> SendPayoutInsertionToTerminal(PayoutCashInsertionModel cash)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri("http://cwmon.ru:444/postrc/");

                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, "api/refillgatewash/payout");
                requestMessage.Content = new StringContent(JsonConvert.SerializeObject(cash), Encoding.UTF8, "application/json");

                HttpResponseMessage result = await httpClient.SendAsync(requestMessage);

                if (!result.IsSuccessStatusCode)
                {
                    string content = await result.Content.ReadAsStringAsync();
                    _logger.LogError($"Ответ от терминала {cash.TerminalCode} не 200: {result.StatusCode} - {content}");
                    throw new CustomStatusCodeException(System.Net.HttpStatusCode.FailedDependency, "Не удалось отправить на терминал значения сдачи", content);
                }

                return result;
            }
            catch(HttpRequestException e)
            {
                _logger.LogError($"Не удалось соединиться с сервисом управления постами {cash.TerminalCode}. {e.GetType()}: {e.Message}");
                throw new CustomStatusCodeException(System.Net.HttpStatusCode.FailedDependency, "Нет связи с сервисом управления постами", $"Не удалось подключиться к сервису упрввления постами. {e.Message}");
            }
        }

        public async Task<HttpResponseMessage> SendNotificationCardsInsertion(TerminalCardsInsertionModel cards)
        {
            var content = new { chatId = "-650370220", body = $"{cards.TerminalCode}: Добавлены карты cards1={cards.Cards1}, cards2={cards.Cards2}" };

            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("http://cwmon.ru/notify/");

            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, "api/notify/message-group");
            requestMessage.Content = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");

            HttpResponseMessage result = await httpClient.SendAsync(requestMessage);

            return result;
        }

        public async Task<HttpResponseMessage> SendCardInsertionToTerminal(TerminalCardsInsertionModel cards)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri("http://cwmon.ru:444/postrc/");

                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, "api/refillgatewash/cards");
                requestMessage.Content = new StringContent(JsonConvert.SerializeObject(cards), Encoding.UTF8, "application/json");

                HttpResponseMessage result = await httpClient.SendAsync(requestMessage);

                if (!result.IsSuccessStatusCode)
                {
                    string content = await result.Content.ReadAsStringAsync();
                    _logger.LogError($"Ответ от терминала {cards.TerminalCode} не 200: {result.StatusCode} - {content}");
                    throw new CustomStatusCodeException(System.Net.HttpStatusCode.FailedDependency, "Не удалось отправить на терминал значения сдачи", content);
                }

                return result;
            }
            catch (HttpRequestException e)
            {
                _logger.LogError($"Не удалось соединиться с сервисом управления постами {cards.TerminalCode}. {e.GetType()}: {e.Message}");
                throw new CustomStatusCodeException(System.Net.HttpStatusCode.FailedDependency, "Нет связи с сервисом управления постами", $"Не удалось подключиться к сервису упрввления постами. {e.Message}");
            }
        }

        public async Task<CurrentCounters> GetCurrentCountersAsync(string terminal)
        {
            if (string.IsNullOrEmpty(terminal))
            {
                _logger.LogError("Код терминала не задан");
                throw new CustomStatusCodeException(System.Net.HttpStatusCode.BadRequest, "Не задан код терминала", "");
            }

            if(!(await _context.Devices.AnyAsync(t => t.Code == terminal)))
            {
                _logger.LogError($"Терминал {terminal} не найден");
                throw new CustomStatusCodeException(System.Net.HttpStatusCode.NotFound, "Не найден терминал", "Терминал по введённому коду не найден");
            }

            string action = await GetTerminalAction(terminal);

            switch (action)
            {
                case "cash":
                    return await GetCurrentPayoutCountersAsync(terminal);
                case "cards":
                    return await GetCurrentCardsCountersAsync(terminal);
                default:
                    _logger.LogError($"Действие для терминала {terminal} не определено");
                    throw new CustomStatusCodeException(System.Net.HttpStatusCode.BadRequest, "Не определён тип счётчиков для терминала", $"Для терминала {terminal} не определён тип счётчиков");
            }
        }

        private async Task<CurrentCounters> GetCurrentPayoutCountersAsync(string terminal)
        {
            try
            {
                CurrentCounters counters = await _context.EventPayouts.Include(o => o.IdpayEventNavigation).ThenInclude(o => o.IddeviceNavigation)
                    .Where(o => o.IdpayEventNavigation.Dtime == (_context.EventPayouts.Include(o => o.IdpayEventNavigation).ThenInclude(o => o.IddeviceNavigation)
                                                                    .Where(d => d.IdpayEventNavigation.IddeviceNavigation.Code == terminal)
                                                                    .Max(w => w.IdpayEventNavigation.Dtime))
                                && o.IdpayEventNavigation.IddeviceNavigation.Code == terminal)
                    .Select(o => new CurrentCounters
                    {
                        DTime = o.IdpayEventNavigation.Dtime,
                        Counters = new Dictionary<string, int>()
                        {
                            { "m10",  o.Inbox5M10},
                            { "b50", o.Inbox1B50 },
                            { "b100", o.Inbox3B100 }
                        }
                    }).FirstOrDefaultAsync();

                return counters;
            }
            catch(Exception e)
            {
                _logger.LogError($"Произошла ошибка при получении текущих счётчиков по сдаче: {e.GetType()} - {e.Message}");
                throw new CustomStatusCodeException(System.Net.HttpStatusCode.InternalServerError, "Произошла в ходе обращения к базе данных", 
                    "При получении текущих значений счётчиков произошла ошибка. Попробуйте позже");
            }
        }

        private async Task<CurrentCounters> GetCurrentCardsCountersAsync(string terminal)
        {
            try
            {
                CurrentCounters counters = await _context.CardCounters.Include(o => o.IddeviceNavigation).Where(o => o.Dtime == (_context.CardCounters.Include(o => o.IddeviceNavigation)
                                                                                                                          .Where(o => o.IddeviceNavigation.Code == terminal).Max(o => o.Dtime))
                                                                                                              && o.IddeviceNavigation.Code == terminal)
                                                                     .Select(o => new CurrentCounters 
                                                                     {
                                                                         DTime = o.Dtime,
                                                                         Counters = new Dictionary<string, int>()
                                                                         {
                                                                             { "dispenser1", o.Dispenser1 },
                                                                             { "dispenser2", o.Dispenser2 }
                                                                         }
                                                                     }).FirstOrDefaultAsync();

                return counters;
            }
            catch (Exception e)
            {
                _logger.LogError($"Произошла ошибка при получении текущих счётчиков по сдаче: {e.GetType()} - {e.Message}");
                throw new CustomStatusCodeException(System.Net.HttpStatusCode.InternalServerError, "Произошла в ходе обращения к базе данных",
                    "При получении текущих значений счётчиков произошла ошибка. Попробуйте позже");
            }
        }

        private async Task<string> GetTerminalAction(string terminal)
        {
            return await _context.Terminals.Include(o => o.IddeviceNavigation).ThenInclude(o => o.IddeviceTypeNavigation).ThenInclude(o => o.DeviceTypeAction)
                .Where(t => t.IddeviceNavigation.Code == terminal &&
                    (t.IddeviceNavigation.IddeviceTypeNavigation.DeviceTypeAction.InsertPayoutCash || t.IddeviceNavigation.IddeviceTypeNavigation.DeviceTypeAction.InsertWashCards))
                .Select(e => e.IddeviceNavigation.IddeviceTypeNavigation.DeviceTypeAction.InsertPayoutCash ? "cash" 
                            : e.IddeviceNavigation.IddeviceTypeNavigation.DeviceTypeAction.InsertWashCards ? "cards" 
                            : null)
                .FirstOrDefaultAsync();
        }

    }

}
