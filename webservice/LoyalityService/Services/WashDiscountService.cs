using LoyalityService.Models;
using LoyalityService.Models.GateWashContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoyalityService.Services
{
    public class WashDiscountService
    {
        private readonly GateWashDbContext _context;
        private readonly ILogger _logger;
        private readonly PostRCCallerService _postRCService;

        public WashDiscountService(GateWashDbContext context, ILogger logger, PostRCCallerService postRC)
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
            // получить пост, который запускали
            Device device = await GetDeviceAsync(long.Parse(call.To));
            
            // новая запись о мойке
            _context.Washings.Add(new Washing
            {
                Dtime = call.When,
                Phone = long.Parse(call.From),
                Iddevice = device.Iddevice,
                Complited = false,
                Discount = discount
            });

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

        /// <summary>
        /// найти девайс по его телефону
        /// </summary>
        /// <param name="phone">Телефон девайса</param>
        /// <returns></returns>
        public async Task<Device> GetDeviceAsync(long phone)
        {
            return await _context.Devices.Where(d => d.Phone == phone).FirstOrDefaultAsync();
        }

        /// <summary>
        /// получить скидку пользователя
        /// </summary>
        /// <param name="phone">Телефон пользователя</param>
        /// <returns></returns>
        public async Task<int> GetCustomerDiscountAsync(long phone)
        {
            int washCount = await _context.Washings.Where(w => w.Phone == phone && w.Complited).CountAsync();

            if (washCount % 5 == 0)
            {
                return 70;
            }

            return 0;
        }

        /// <summary>
        /// получить код девайса по его телефону
        /// </summary>
        /// <param name="phone">Телефон девайса</param>
        /// <returns></returns>
        public async Task<string> GetDeviceCodeAsync(long phone)
        {
            var device = await _context.Devices.Where(d => d.Phone == phone).FirstOrDefaultAsync();
            return device.Code;
        }

        /// <summary>
        /// запустить пост через сервис PostRC
        /// </summary>
        /// <param name="call">Параметры входящего вызова</param>
        public async void StartPostAsync(IncomeCallModel call)
        {
            // получить скидку и код поста по входящим параметрам вызова
            string deviceCode = await GetDeviceCodeAsync(long.Parse(call.To));
            int discount = await GetCustomerDiscountAsync(long.Parse(call.From));

            // запуск поста
            var response = await _postRCService.StartPostAsync(new StartPostParameters { DeviceCode = deviceCode, Discount = discount });

            // если удачно, записать запись о мойке
            if (response.IsSuccessStatusCode)
            {
                WriteWashingAsync(call, discount);
            }            
        }
    }
}
