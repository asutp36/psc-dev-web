using LoyalityService.Models;
using LoyalityService.Models.GateWashContext;
using LoyalityService.Models.WashLoyality;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace LoyalityService.Services
{
    public class WashDiscountService : IDiscountManager
    {
        private readonly WashLoyalityDbContext _context;
        private readonly ILogger _logger;
        private readonly PostRCCallerService _postRCService;

        public WashDiscountService(WashLoyalityDbContext context, ILogger<WashDiscountService> logger, PostRCCallerService postRC)
        {
            _context = context;
            _logger = logger;
            _postRCService = postRC;
        }

        /// <summary>
        /// записать новую мойку
        /// </summary>
        /// <param name="call">Вызов запуска мойки</param>
        private async void WriteWashingAsync(IncomeCallModel call, int discount)
        {
            //// получить пост, который запускали
            //Device device = await GetDeviceAsync(long.Parse(call.To));
            
            //// новая запись о мойке
            //_context.Washings.Add(new Washing
            //{
            //    Dtime = call.When,
            //    Phone = long.Parse(call.From),
            //    Iddevice = device.Iddevice,
            //    Complited = false,
            //    Discount = discount
            //});

            // сохранение изменений
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e)
            {
                _logger.LogError($"| LoyalityService.WriteWashing | {nameof(e.GetType)}: {e.Message}");
            }
            catch (DbUpdateException e)
            {
                _logger.LogError($"| LoyalityService.WriteWashing | {nameof(e.GetType)}: {e.Message}");
            }
            finally 
            {
                _logger.LogError($"| LoyalityService.WriteWashing | Мойка не записалась, параметры: {JsonConvert.SerializeObject(call)}, скидка {discount}");
            }
        }

        public async Task<int> CalculateDiscountAsync(string terminalCode, long phone)
        {
            // все скидки, под которые попадает этот телефон (код скидки: величина)
            Dictionary<string, int> availibleDiscounts = new Dictionary<string, int>();
            


            return availibleDiscounts.Max().Value;
        }

        /// <summary>
        /// рассчитать скидку типа "каждая энная мойка"
        /// </summary>
        private async Task CalculateEachNWashDicsount(string terminalCode, long phone) 
        {
            // получил группу, в которую входит терминал
            Group group = await this.GetWashGroupByTerminalCodeAsync(terminalCode);

            // IdconditionNavigation - условия для каждой энной мойки
            var conditions = _context.Promotions
                        .Where(o => o.IdgroupNavigation.Code == group.Code)
                        .Select(o => new
                        {
                            Discount = o.Discount,
                            EachN = o.IdconditionNavigation.EachN,
                            Days = o.IdconditionNavigation.Days
                        })
                        .AsEnumerable();

        }

        /// <summary>
        /// рассчитать скидку типа "счастливые часы"
        /// </summary>
        private void CalculateHappyHourDiscount() { }

        /// <summary>
        /// рассчитать скидку типа "праздничный день"
        /// </summary>
        private void CalculateHolidayDiscount() { }

        /// <summary>
        /// рассчитать скидку типа "vip клиент"
        /// </summary>
        private void CalculateVipDiscount() { }

        private async Task<Group> GetWashGroupByTerminalCodeAsync(string terminalCode)
        {
            var group = await _context.Terminals.Where(o => o.IddeviceNavigation.Code == terminalCode)
                .Select(o => o.IdwashNavigation.IdgroupNavigation)
                .FirstOrDefaultAsync();

            return group;
        }

        public Task WriteWashingAsync(string terminaCode, long phone)
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetTerminalCodeByPhoneAsync(long phone)
        {
            string code = await _context.Terminals.Where(t => t.Phone == phone)
                            .Select(t => t.IddeviceNavigation.Code)
                            .FirstOrDefaultAsync();

            return code;
        }
    }
}
