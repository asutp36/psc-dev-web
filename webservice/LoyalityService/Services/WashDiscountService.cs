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
using System.Data;

namespace LoyalityService.Services
{
    public class WashDiscountService : IDiscountManager
    {
        private readonly WashLoyalityDbContext _context;
        private readonly ILogger _logger;
        private readonly IPostRCCaller _postRCService;

        public WashDiscountService(WashLoyalityDbContext context, ILogger<WashDiscountService> logger, IPostRCCaller postRC)
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
            _logger.LogInformation("logging test");
            // получаю группу, в которую входит терминал
            Group group = await GetWashGroupByTerminalCodeAsync(terminalCode);

            // все скидки, под которые попадает этот телефон (код скидки: величина)
            Dictionary<string, int> availibleDiscounts = new Dictionary<string, int>();

            int eachNwashDiscount = await CalculateEachNWashDicsount(group.Code, phone);
            availibleDiscounts.Add("eachN", eachNwashDiscount);

            int happyHourDiscount = await CalculateHappyHourDiscount(group.Code);
            availibleDiscounts.Add("happyHour", happyHourDiscount);

            int holidayDiscount = await CalculateHolidayDiscount(group.Code);
            availibleDiscounts.Add("holiday", holidayDiscount);

            int vipDiscount = await CalculateVipDiscount(group.Code, phone);
            availibleDiscounts.Add("vip", vipDiscount);

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
        /// <returns>Величина скидки, которая положена клиенту, или 0</returns>
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
        /// Рассчитать скидку "счастливые часы"
        /// </summary>
        /// <param name="groupCode">Код группы моек</param>
        /// <returns>Величина скидки, если сейчас идёт счастливый час, или 0</returns>
        private async Task<int> CalculateHappyHourDiscount(string groupCode) 
        {
            var promotion = await _context.Promotions.Where(o => o.IdgroupNavigation.Code == groupCode
                                                        && o.HappyHourCondition != null
                                                        && o.HappyHourCondition.HourBegin <= DateTime.Now.Hour
                                                        && o.HappyHourCondition.HourEnd > DateTime.Now.Hour)
                                                        .FirstOrDefaultAsync();

            // если найдена акция, которая сейчас идёт
            if(promotion != null)
            {
                return promotion.Discount;
            }

            return 0;
        }

        /// <summary>
        /// Рассчитать скидку типа "праздничный день"
        /// </summary>
        /// <param name="groupCode">Код группы моек</param>
        /// <returns>Величина скидки, если сегодня праздничный день, или 0</returns>
        private async Task<int> CalculateHolidayDiscount(string groupCode) 
        {
            try
            {
                int maxDiscount = await _context.Promotions.Where(o => o.IdgroupNavigation.Code == groupCode
                                                            && o.HolidayCondition != null
                                                            && o.HolidayCondition.Date == DateTime.Now.Date)
                                                     .MaxAsync(o => o.Discount);
                return maxDiscount;
            }
            catch (InvalidOperationException e)
            {
                _logger.LogError($"| WashDiscountService.CalculateHolidayDiscount | Не найдены скидки в праздничные дня для группы моек {groupCode}. InvalidOperationException: {e.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Рассчитать скидку типа "vip клиент"
        /// </summary>
        /// <param name="groupCode">Код группы моек</param>
        /// <param name="phone">Номер телефона клиента</param>
        /// <returns>Величина скидки, если клент есть в списке vip, или 0</returns>
        private async Task<int> CalculateVipDiscount(string groupCode, long phone) 
        {
            var promotion = await _context.Promotions.Where(o => o.IdgroupNavigation.Code == groupCode
                                                        && o.VipCondition != null
                                                        && o.VipCondition.Phone == phone)
                                              .FirstOrDefaultAsync();

            // если найден этот телефон в списках vip
            if(promotion != null)
            {
                return promotion.Discount;
            }

            return 0;
        }

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

            // если терминал не найден - выкинуть KeyNotFoundException
            if (string.IsNullOrEmpty(code))
            {
                _logger.LogError($"Не удалось найти терминал по номеру телефона {phone}");
                throw new KeyNotFoundException($"Не удалось найти терминал по номеру телефона {phone}");
            }

            return code;
        }

        public async Task<WashingModel> GetClientLastWashing(long clientPhone)
        {
            var washing = await _context.Washings.Where(o => o.IdclientNavigation.Phone == clientPhone)
                .Select(o => new WashingModel
                {
                    ClientPhone = o.IdclientNavigation.Phone,
                    DTime = o.Dtime,
                    Device = o.IddeviceNavigation.Name,
                    Program = o.IdprogramNavigation.Name,
                    Amount = o.Amount,
                    Discount = o.Discount
                })
                .FirstOrDefaultAsync();

            return washing;
        }

        public void CalcWashingsBeforeDiscount(long clientPhone)
        {
            
        }
    }
}
