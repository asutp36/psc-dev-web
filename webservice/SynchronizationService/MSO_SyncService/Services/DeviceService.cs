using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MSO.SyncService.Exceptions;
using MSO_SyncService.Models.WashCompanyDb;

namespace MSO.SyncService.Services
{
    public class DeviceService
    {
        private readonly WashCompanyDbContext _context;
        private readonly ILogger _logger;

        public DeviceService(WashCompanyDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> IsExists(string deviceCode)
        {
            try
            {
                if (deviceCode.IsNullOrEmpty())
                {
                    throw new CustomStatusCodeException("Не задан код девайса", 400);
                }

                return await _context.Devices.AnyAsync(d => d.Code == deviceCode);
            }
            catch (CustomStatusCodeException e)
            {
                throw e;
            }
            catch (Exception e) 
            {
                _logger.LogError($"DeviceService.IsExists: {e.GetType()}: {e.Message}");
                throw new CustomStatusCodeException("При обращении к базе данных произошла ошибка", 513);
            }
        }
    }
}
