using GateWashSyncService.Models.GateWash;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashSyncService.Services
{
    public class EventKindsService
    {
        private readonly ILogger<EventKindsService> _logger;
        private readonly GateWashDbContext _context;
        public EventKindsService(ILogger<EventKindsService> logger, GateWashDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// По коду проверить, существует ли тип события
        /// </summary>
        /// <param name="code">Код типа события</param>
        /// <returns>bool</returns>
        public async Task<bool> CheckIfExistsByCodeAsync(string code)
        {
            if (String.IsNullOrEmpty(code))
                return false;
            return await _context.EventKind.AnyAsync(o => o.Code == code);
        }

        /// <summary>
        /// Получить id типа события на сервере
        /// </summary>
        /// <param name="code">Код типа события</param>
        /// <returns>Id типа события на сервере</returns>
        public async Task<int> GetIdByCode(string code)
        {
            return await _context.EventKind.Where(o => o.Code == code).Select(o => o.IdeventKind).FirstOrDefaultAsync();
        }
    }
}
