using LoyalityService.Models;
using LoyalityService.Models.GateWashContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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

        public WashDiscountService(GateWashDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// записать новую мойку
        /// </summary>
        /// <param name="call">Вызов запуска мойки</param>
        private async void WriteWashingAsync(IncomeCallModel call)
        {
            Device device = await GetDeviceAsync(long.Parse(call.To));
            _context.Washings.Add(new Washing
            {
                Dtime = call.DTime,
                Phone = long.Parse(call.From),
                Iddevice = device.Iddevice
            });

            try
            {
                _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e) 
            {
                _logger.LogError($"| LoyalityService.WriteWashing | {nameof(e.GetType)}: {e.Message}");
            }
            catch (DbUpdateException e) 
            {
                _logger.LogError($"| LoyalityService.WriteWashing | {nameof(e.GetType)}: {e.Message}");
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
        public async Task<int> GetDiscountAsync(long phone)
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
    }
}
