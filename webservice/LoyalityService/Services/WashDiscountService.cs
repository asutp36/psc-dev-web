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
            // получаю группу, в которую входит терминал
            Group group = await GetWashGroupByTerminalCodeAsync(terminalCode);
            // все скидки, под которые попадает этот телефон (код скидки: величина)
            Dictionary<string, int> availibleDiscounts = new Dictionary<string, int>();

            int eachNwashDiscount = await CalculateEachNWashDicsount(group.Code, phone);
            availibleDiscounts.Add("eachN", eachNwashDiscount);

            int happyHourDiscount = await CalculateHappyHourDiscount(group.Code);
            availibleDiscounts.Add("happyHour", happyHourDiscount);

            return availibleDiscounts.Max(p => p.Value);
        }

        /// <summary>
        /// рассчитать скидку типа "каждая энная мойка"
        /// </summary>
        /// <param name="groupCode">Код группы</param>
        /// <param name="clientPhone">Номер телефона клиента</param>
        /// <returns>Максимальная скидка</returns>
        private async Task<int> CalculateEachNWashDicsount(string groupCode, long clientPhone) 
        {
            // определить условия акций для группы 
            IEnumerable<EachNWashPromotionCondition> conditions = await _context.Promotions
                        .Where(o => o.IdgroupNavigation.Code == groupCode
                        && o.EachNwashCondition != null)
                        .Select(o => new EachNWashPromotionCondition
                        {
                            Discount = o.Discount,
                            EachN = o.EachNwashCondition.EachN,
                            Days = o.EachNwashCondition.Days
                        })
                        .ToListAsync();

            // перебирем каждое условие, высчитываем максимальную скидку
            int maxDiscount = 0;
            foreach(var c in conditions)
            {
                int currentDiscount = CheckEachNWashCondition(c, clientPhone);
                if (currentDiscount > maxDiscount)
                    maxDiscount = currentDiscount;
            }

            return maxDiscount;
        }

        /// <summary>
        /// Проверить, подходит ли клиент под условия акции, вернуть полагающуюся скидку
        /// </summary>
        /// <param name="condition">Условие акции</param>
        /// <param name="clientPhone">Номер телефона клиента</param>
        /// <returns>Скидка, которая положена клиенту</returns>
        private int CheckEachNWashCondition(EachNWashPromotionCondition condition, long clientPhone)
        {
            // посчитать все мойки клиента за последние Days (из условия скидки)
            int clientWashingsCount = _context.Washings.Count(o => o.IdclientNavigation.Phone == clientPhone 
                                                                && o.Dtime.Date <= DateTime.Now.Date.AddDays(-condition.Days));

            // если количество моек > 0 и эта мойка будет энной, скидка будет из условия или 0
            if(clientWashingsCount > 0 && clientWashingsCount + 1 % condition.EachN == 0)
            {
                return condition.Discount;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// рассчитать скидку "счастливые часы"
        /// </summary>
        /// <param name="groupCode">Код группы</param>
        /// <returns>Максимальная доступная скидка</returns>
        private async Task<int> CalculateHappyHourDiscount(string groupCode) 
        {
            int maxDiscount = await _context.Promotions.Where(o => o.IdgroupNavigation.Code == groupCode
                                                        && o.HappyHourCondition != null
                                                        && o.HappyHourCondition.HourBegin <= DateTime.Now.Hour
                                                        && o.HappyHourCondition.HourEnd > DateTime.Now.Hour)
                                                .MaxAsync(o => o.Discount);
            return maxDiscount;
        }

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
