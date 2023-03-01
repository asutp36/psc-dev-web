using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MSO.SyncService.Exceptions;
using MSO.SyncService.Models.WashCompanyDb;

namespace MSO.SyncService.Services
{
    public class ModeService
    {
        private readonly WashCompanyDbContext _context;
        private readonly ILogger<ModeService> _logger;

        public ModeService(WashCompanyDbContext context, ILogger<ModeService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> IsExistsAsync(string modeCode)
        {
            try
            {
                if (modeCode.IsNullOrEmpty())
                {
                    throw new CustomStatusCodeException("Не задан код режима", 400);
                }

                return await _context.Modes.AnyAsync(ct => ct.Code == modeCode);
            }
            catch (CustomStatusCodeException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                _logger.LogError($"ModeService.IsExists: {e.GetType()}: {e.Message}");
                throw new CustomStatusCodeException("При обращении к базе данных произошла ошибка", 513);
            }
        }

        public async Task<int> GetIdByCode(string modeCode)
        {
            try
            {
                if (modeCode.IsNullOrEmpty())
                {
                    throw new CustomStatusCodeException("Не задан код режима", 400);
                }

                return await _context.Modes.Where(m => m.Code == modeCode).Select(m => m.Idmode).FirstOrDefaultAsync();
            }
            catch (CustomStatusCodeException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                _logger.LogError($"ModeService.GetIdByCode: {e.GetType()}: {e.Message}");
                throw new CustomStatusCodeException("При обращении к базе данных произошла ошибка", 513);
            }
        }
    }
}
