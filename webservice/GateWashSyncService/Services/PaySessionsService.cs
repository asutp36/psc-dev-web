using GateWashSyncService.Models.GateWash;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashSyncService.Services
{
    public class PaySessionsService
    {
        private readonly ILogger<PaySessionsService> _logger;
        private readonly GateWashDbContext _context;

        public PaySessionsService(ILogger<PaySessionsService> logger, GateWashDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Проверить, существует ли нужная сессия на посту
        /// </summary>
        /// <param name="idSessionOnPost">ID сессии на посту</param>
        /// <param name="deviceCode">Код поста</param>
        /// <returns>bool</returns>
        public async Task<bool> CheckIfExistsAsync(int idSessionOnPost, string deviceCode)
        {
            if (idSessionOnPost < 0 || string.IsNullOrEmpty(deviceCode))
                return false;

            return await _context.PaySession.Include(o => o.IddeviceNavigation).AnyAsync(o => o.IdsessionOnPost == idSessionOnPost && o.IddeviceNavigation.Code == deviceCode);
        }

        /// <summary>
        /// Получить id платёжной сессии по её локальному id и код девайса
        /// </summary>
        /// <param name="idSessionOnPost">Локальный id сессии на посту</param>
        /// <param name="deviceCode">Код девайса</param>
        /// <returns>id сессии в базе на сервере</returns>
        public async Task<int> GetIdyLocalIdAndDevice(int idSessionOnPost, string deviceCode)
        {
            return await _context.PaySession.Where(o => o.IdsessionOnPost == idSessionOnPost && o.IddeviceNavigation.Code == deviceCode).Select(o => o.IdpaySession).FirstOrDefaultAsync();
        }
    }
}
