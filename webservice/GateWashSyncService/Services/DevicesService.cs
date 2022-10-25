using GateWashSyncService.Models.GateWash;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashSyncService.Services
{
    public class DevicesService
    {
        private readonly ILogger<DevicesService> _logger;
        private readonly GateWashDbContext _context;
        public DevicesService(ILogger<DevicesService> logger, GateWashDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// По коду проверить, существует  ли девайс
        /// </summary>
        /// <param name="code">Код девайса</param>
        /// <returns>bool</returns>
        public async Task<bool> CheckIfExistsByCodeAsync(string code)
        {
            if (string.IsNullOrEmpty(code))
                return false;

            return await _context.Device.AnyAsync(o => o.Code == code);
        }

        /// <summary>
        /// Получить id по коду
        /// </summary>
        /// <param name="code">Код девайса</param>
        /// <returns>Id девайса на сервере</returns>
        public async Task<int> GetIdByCode(string code)
        {
            return await _context.Device.Where(o => o.Code == code).Select(o => o.Iddevice).FirstOrDefaultAsync();
        }
    }
}
