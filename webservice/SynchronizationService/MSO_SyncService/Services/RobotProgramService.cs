using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MSO.SyncService.Exceptions;
using MSO.SyncService.Models.WashCompanyDb;

namespace MSO.SyncService.Services
{
    public class RobotProgramService
    {
        private readonly WashCompanyDbContext _context;
        private readonly ILogger<RobotProgramService> _logger;

        public RobotProgramService(WashCompanyDbContext context, ILogger<RobotProgramService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> IsExistsAsync(string programCode)
        {
            try
            {
                if (programCode.IsNullOrEmpty())
                {
                    throw new CustomStatusCodeException("Не задан код программы", 400);
                }

                return await _context.RobotPrograms.AnyAsync(p => p.Code == programCode);
            }
            catch (CustomStatusCodeException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                _logger.LogError($"RobotProgramService.IsExists: {e.GetType()}: {e.Message}");
                throw new CustomStatusCodeException("При обращении к базе данных произошла ошибка", 513);
            }
        }

        public async Task<int> GetIdByCodeAsync(string programCode)
        {
            return await _context.RobotPrograms.Where(p => p.Code == programCode).Select(p => p.IdrobotProgram).FirstOrDefaultAsync();
        }
    }
}
