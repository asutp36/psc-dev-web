using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MSO.SyncService.Exceptions;
using MSO.SyncService.Models.WashCompanyDb;

namespace MSO.SyncService.Services
{
    public class EventKindService
    {
        private readonly WashCompanyDbContext _context;
        private readonly ILogger<EventKindService> _logger;

        public EventKindService(WashCompanyDbContext context, ILogger<EventKindService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> IsExistsAsync(string eventKindCode)
        {
            try
            {
                if (eventKindCode.IsNullOrEmpty())
                {
                    throw new CustomStatusCodeException("Не задан код события", 400);
                }

                return await _context.EventKinds.AnyAsync(ek => ek.Code == eventKindCode);
            }
            catch (CustomStatusCodeException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                _logger.LogError($"EventKindService.IsExists: {e.GetType()}: {e.Message}");
                throw new CustomStatusCodeException("При обращении к базе данных произошла ошибка", 513);
            }
        }

        public async Task<int> GetIdByCodeAsync(string eventKindCode)
        {
            return await _context.EventKinds.Where(ek => ek.Code == eventKindCode).Select(ek => ek.IdeventKind).FirstOrDefaultAsync();
        }
    }
}
