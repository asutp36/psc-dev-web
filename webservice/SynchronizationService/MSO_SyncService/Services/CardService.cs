using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MSO.SyncService.Exceptions;
using MSO.SyncService.Models.WashCompanyDb;

namespace MSO.SyncService.Services
{
    public class CardService
    {
        private readonly WashCompanyDbContext _context;
        private readonly ILogger<CardService> _logger;

        public CardService(WashCompanyDbContext context, ILogger<CardService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> IsCardTypeExistsAsync(string cardTypeCode)
        {
            try
            {
                if (cardTypeCode.IsNullOrEmpty())
                {
                    throw new CustomStatusCodeException("Не задан код типа карты", 400);
                }

                return await _context.CardTypes.AnyAsync(ct => ct.Code == cardTypeCode);
            }
            catch (CustomStatusCodeException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                _logger.LogError($"CardService.IsCardTypeExists: {e.GetType()}: {e.Message}");
                throw new CustomStatusCodeException("При обращении к базе данных произошла ошибка", 513);
            }
        }

        public async Task<bool> IsCardExistsAsync(string cardNum)
        {
            try
            {
                if (cardNum.IsNullOrEmpty())
                {
                    throw new CustomStatusCodeException("Не задан номер карты", 400);
                }

                return await _context.Cards.AnyAsync(c => c.CardNum == cardNum);
            }
            catch (CustomStatusCodeException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                _logger.LogError($"CardService.IsCardExists: {e.GetType()}: {e.Message}");
                throw new CustomStatusCodeException("При обращении к базе данных произошла ошибка", 513);
            }
        }
    }
}
